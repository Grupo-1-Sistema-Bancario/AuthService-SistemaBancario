using System.ComponentModel.DataAnnotations;

namespace AuthServiceIN6BV.Application.DTOs.Email;

public class ResetPasswordDto
{
    public string Token { get; set; } = string.Empty;
    
    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
}