using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services.ServiceDef
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        public JwtService(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateToken(User user, IList<string> roles)
        {
            var jwtKey = _config["Jwt:Key"]
             ?? throw new Exception("JWT Key is missing from configuration");
            var jwtIssuer = _config["Jwt:Issuer"]
                            ?? throw new Exception("JWT Issuer is missing from configuration");
            var jwtAudience = _config["Jwt:Audience"]
                              ?? throw new Exception("JWT Audience is missing from configuration");
            var expireMinutes = int.TryParse(_config["Jwt:ExpireMinutes"], out var minutes) ? minutes : 60;


            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName),
                new Claim("fullname",user.FullName ?? "")
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}