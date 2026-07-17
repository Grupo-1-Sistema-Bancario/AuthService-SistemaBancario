using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AuthServiceSistemaBancario.Application.Interfaces;

namespace AuthServiceSistemaBancario.Application.Services;

public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    public async Task SendEmailVerificationAsync(string email, string username, string token)
    {
        var subject = "Verify your email address";
        var verificationUrl = $"{configuration["AppSettings:FrontendUrl"]}/verify-email?token={token}";

        var body = $@"
            <h2>Welcome {username}!</h2>
            <p>Please verify your email address by clicking the link below:</p>
            <a href='{verificationUrl}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                Verify Email
            </a>
            <p>If you cannot click the link, copy and paste this URL into your browser:</p>
            <p>{verificationUrl}</p>
            <p>This link will expire in 24 hours.</p>
            <p>If you didn't create an account, please ignore this email.</p>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetAsync(string email, string username, string token)
    {
        var subject = "Reset your password";
        var resetUrl = $"{configuration["AppSettings:FrontendUrl"]}/reset-password?token={token}";

        var body = $@"
            <h2>Password Reset Request</h2>
            <p>Hello {username},</p>
            <p>You requested to reset your password. Click the link below to reset it:</p>
            <a href='{resetUrl}' style='background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                Reset Password
            </a>
            <p>If you cannot click the link, copy and paste this URL into your browser:</p>
            <p>{resetUrl}</p>
            <p>This link will expire in 1 hour.</p>
            <p>If you didn't request this, please ignore this email and your password will remain unchanged.</p>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string email, string username)
    {
        var subject = "Welcome to AuthDotnet!";

        var body = $@"
            <h2>Welcome to AuthDotnet, {username}!</h2>
            <p>Your account has been successfully verified and activated.</p>
            <p>You can now enjoy all the features of our platform.</p>
            <p>If you have any questions, feel free to contact our support team.</p>
            <p>Thank you for joining us!</p>
        ";

        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpSettings = configuration.GetSection("SmtpSettings");

        try
        {
            // Verificar si el email está habilitado
            var enabled = bool.Parse(smtpSettings["Enabled"] ?? "true");
            if (!enabled)
            {
                logger.LogInformation("Email disabled in configuration. Skipping send");
                return;
            }

            // Validar configuración
            var fromEmail = smtpSettings["FromEmail"];
            var fromName = smtpSettings["FromName"];
            var apiKey = smtpSettings["BrevoApiKey"] ?? smtpSettings["ApiKey"] ?? smtpSettings["Password"];

            if (string.IsNullOrEmpty(apiKey))
            {
                logger.LogError("Brevo API Key (BrevoApiKey, ApiKey, or Password) is not properly configured");
                throw new InvalidOperationException("Brevo API Key is not properly configured");
            }

            var payload = new
            {
                sender = new { name = fromName, email = fromEmail },
                to = new[] { new { email = to, name = "" } },
                subject = subject,
                htmlContent = body
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var brevoUrl = smtpSettings["BrevoUrl"] ?? "https://api.brevo.com/v3/smtp/email";
            using var request = new HttpRequestMessage(HttpMethod.Post, brevoUrl);
            request.Headers.Add("api-key", apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = content;

            var timeoutMs = int.Parse(smtpSettings["Timeout"] ?? "30000");
            using var cts = new CancellationTokenSource(timeoutMs);

            var response = await _httpClient.SendAsync(request, cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                logger.LogError("Brevo REST API returned error: {StatusCode} - {Error}", response.StatusCode, errorResponse);
                throw new InvalidOperationException($"Failed to send email via Brevo REST API: {response.StatusCode} - {errorResponse}");
            }

            logger.LogInformation("Email sent successfully via Brevo REST API");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email via Brevo REST API");

            // Verificar si usar fallback
            var useFallback = bool.Parse(smtpSettings["UseFallback"] ?? "false");
            if (useFallback)
            {
                logger.LogWarning("Using email fallback");
                return; // No fallar, solo logear
            }

            throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
        }
    }
}