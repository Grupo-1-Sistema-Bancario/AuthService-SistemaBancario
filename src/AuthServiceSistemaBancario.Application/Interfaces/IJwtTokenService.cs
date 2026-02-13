
using AuthServiceSistemaBancario.Domain.Entities;

namespace AuthServiceSistemaBancario.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}