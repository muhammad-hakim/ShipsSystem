using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ships_System.DAL;

namespace Ships_System.BL
{
    public class AgentService : IAgentService
    {
        private readonly UnitOfWork unitOfWork;

        public AgentService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Agent AddAgent(Agent agent)
        {
            return unitOfWork.Agents.Add(agent);
        }

        public async Task<bool> DeleteAgentAsync(int agentId)
        {
            return await unitOfWork.Agents.DeleteAsync(agentId);
        }

        public async Task<List<Agent>> GetAllAgentsAsync()
        {
            return await unitOfWork.Agents.GetAsync();
        }

        public async Task<Agent> GetAgentByIdAsync(int id)
        {
            return await unitOfWork.Agents.GetByIdAsync(id);
        }

        public async Task<Agent> UpdateAgentAsync(Agent agent)
        {
            return await unitOfWork.Agents.UpdateAsync(agent.AgentId, agent);
        }
    }
}
