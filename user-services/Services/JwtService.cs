using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace user_services.Services
{
    public class JwtService
    {
        private const string SecretKey = "your-secret-key-hahahahahahahahaha"; 
        private readonly SymmetricSecurityKey _signingKey;

        public JwtService()
        {
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        }

        public string GenerateJwtToken(string userId, string email)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "user-service",
                audience: "microservices",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
