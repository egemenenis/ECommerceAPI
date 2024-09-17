using ECommerceApi.API.Entites;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceApi.Services
{
    public class TokenService
    {
        public static string GenerateToken(Account account, IConfiguration configuration)
        {
            byte[] jwtkey = Encoding.UTF8.GetBytes(configuration["JwtOptions:Key"]);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(jwtkey);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            string issuer = "site.com";
            string audience = "site.com";



            List<Claim> claims = new List<Claim>
                    {
                        new Claim("id", account.Id.ToString()),
                        new Claim("type", ((int)account.Type).ToString()),
                        new Claim(ClaimTypes.Name, account.Username),
                        new Claim(ClaimTypes.Role, account.Type.ToString()),
                    };


            JwtSecurityToken jwtSecurityToken =
                new JwtSecurityToken(issuer, audience, claims, expires: DateTime.Now.AddDays(30), signingCredentials: credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }
    }
}
