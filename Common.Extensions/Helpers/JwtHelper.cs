using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Common.Extensions.Helpers;

public static class JwtHelper
{
    public static string? GetUsernameFromToken(string jwtToken, string secret)
    {
        IdentityModelEventSource.ShowPII = true;

        TokenValidationParameters validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        var principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out _);

        return principal.FindFirst("id")?.Value;
    }
    
    public static string? GetJwtClaim(string token, string claimName)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            var claimValue = securityToken.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
            return claimValue;
        }
        catch (Exception)
        {
            return null;
        }
    }
}