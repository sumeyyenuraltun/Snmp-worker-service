using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Snmp.Business.Abstract.Security;
using Snmp.Business.DTOs.User;
using Snmp.Common.Configuration;
using SNMP.ENTITY.Concrete;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Snmp.Business.Concrete.Security
{
    public class JWTService : IJWTService
    {
        private readonly JWTSettings _jwtSettings;

        public JWTService(IOptions<JWTSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        public string CreateToken(UserAuthDTO userAuthDTO)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userAuthDTO.Id.ToString()),
                new Claim(ClaimTypes.Name, userAuthDTO.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, userAuthDTO.RoleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }
    }
}
