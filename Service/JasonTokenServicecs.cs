using E_Commerce.Interface;
using E_Commerce.Types;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using System.Text;

namespace E_Commerce.Service;

    public class JasonTokenServicecs:IJasonToken
    {
        private readonly string _secretkey;
        public JasonTokenServicecs(IConfiguration configuration)
        {
        _secretkey = configuration["JWT:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is missing in configuration.");

    }

    public string CreateToken(Guid Id, string Email, string Name, Role role)
        {

        try {

            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretkey);
            var tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
              new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.Name, Name),
                new Claim(ClaimTypes.Role, role.ToString())
          ]),
                Expires = DateTime.UtcNow.AddHours(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenhandler.CreateToken(tokendescriptor);
            return tokenhandler.WriteToken(token);
                }

        catch (Exception ex)
        {
            throw new Exception("Server error: " + ex.Message);
        }
       
    }

        public Guid VerifyToken(string token)
        {
        try
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretkey);
            var validtoken = new TokenValidationParameters()
            {

                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)

            };

            var principal = tokenhandler.ValidateToken(token, validtoken, out var validatedToken);
            var useridclaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (useridclaim != null)
            {
                return new Guid(useridclaim.Value);
            }

            else
            {
                throw new Exception("User ID not found in token.");


            }
        }
        catch (Exception ex)
        {


            throw new Exception("Token validation failed: " + ex.Message);
        }


            throw new NotImplementedException();
        }
    }

