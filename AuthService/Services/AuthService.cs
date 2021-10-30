using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services
{
    public class AuthService
    {
        private readonly IInsuranceAgents _agents;

        private readonly AppSettings _appSettings;

        public AuthService(IInsuranceAgents agents, IOptions<AppSettings> appSettings)
        {
            _agents = agents;
            _appSettings = appSettings.Value;
        }

        public string Authenticate(string login, string pwd)
        {
            var agent = _agents.FindByLogin(login);

            if (agent == null)
                return null;

            if (!agent.PasswordMatches(pwd))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new("sub", agent.Login),
                    new(ClaimTypes.Name, agent.Login),
                    new(ClaimTypes.Role, "SALESMAN"),
                    new(ClaimTypes.Role, "USER"),
                    new("avatar", agent.Avatar),
                    new("userType", "SALESMAN"),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public InsuranceAgentDto AgentFromLogin(string login)
        {
            return _agents.FindByLogin(login);
        }
    }
}