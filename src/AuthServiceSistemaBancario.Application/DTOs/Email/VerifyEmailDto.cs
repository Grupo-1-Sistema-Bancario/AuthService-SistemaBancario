using System.ComponentModel.DataAnnotations;

namespace AuthServiceSistemaBancario.Application.DTOs.Email;

public class VerifyEmailDto
{
    [Required]
    public string Token { get; set; } = string.Empty;
}