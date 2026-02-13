
using AuthServiceIN6BV.Domain.Entities;

namespace AuthServiceIN6BV.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}