using AuthService.Dtos;

namespace AuthService.Services
{
    public interface IInsuranceAgents
    {
        void Add(InsuranceAgentDto agentDto);

        InsuranceAgentDto FindByLogin(string login);
    }
}