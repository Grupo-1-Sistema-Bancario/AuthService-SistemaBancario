using System.ComponentModel.DataAnnotations;

namespace AuthServiceSistemaBancario.Application.DTOs.Email;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}