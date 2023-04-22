using BlogV2.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace blogV2.Services
{
    public class TokenService
    {
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Subject = new ClaimsIdentity(new Claim []
                {
                    new (ClaimTypes.Name, value: "saulocosta"),
                    new (ClaimTypes.Role, value: "user"),
                    new (ClaimTypes.Role, value: "admin"),
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
