using System.Collections.Concurrent;
using System.Collections.Generic;
using AuthService.Controllers;
using AuthService.Dtos;
using AuthService.Services;

namespace AuthService.DataAccess
{
    public class InsuranceAgentsInMemoryDb : IInsuranceAgents
    {
        private readonly IDictionary<string, InsuranceAgentDto> _db = new ConcurrentDictionary<string, InsuranceAgentDto>();

        public InsuranceAgentsInMemoryDb()
        {
            Add(new InsuranceAgentDto("jimmy.solid", "secret", "static/avatars/jimmy_solid.png", new List<string>() {"TRI", "HSI", "FAI", "CAR"}));
            Add(new InsuranceAgentDto("danny.solid", "secret", "static/avatars/danny.solid.png", new List<string>() {"TRI", "HSI", "FAI", "CAR"}));
            Add(new InsuranceAgentDto("admin", "admin", "static/avatars/admin.png", new List<string>() {"TRI", "HSI", "FAI", "CAR"}));
        }

        public void Add(InsuranceAgentDto agentDto)
        {
            _db[agentDto.Login] = agentDto;
        }

        public InsuranceAgentDto FindByLogin(string login) => _db[login];
    }
}