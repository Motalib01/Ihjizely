using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ihjezly.Application.Abstractions.Authentication;
using Microsoft.IdentityModel.Tokens;
using Ihjezly.Domain.Users;

public class JwtService : IJwtService
{
    private readonly string _secret = "super_secure_secret_key_which_is_long_enough";
    private readonly string _issuer = "ihjezly-api";

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName ),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);


    public bool VerifyPassword(string hashedPassword, string plainPassword) =>
        BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
}