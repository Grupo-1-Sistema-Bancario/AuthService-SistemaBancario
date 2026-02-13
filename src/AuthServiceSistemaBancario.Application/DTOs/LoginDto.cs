using System.ComponentModel.DataAnnotations;

namespace AuthServiceIN6BV.Application.DTOs;

public class LoginDto
{
    [Required]
    public string EmailOrUsername { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}