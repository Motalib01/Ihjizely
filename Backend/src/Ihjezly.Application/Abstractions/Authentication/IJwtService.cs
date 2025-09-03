using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Users;

namespace Ihjezly.Application.Abstractions.Authentication;

public interface IJwtService
{
    
        string GenerateToken(User user);
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string plainPassword);
    

}