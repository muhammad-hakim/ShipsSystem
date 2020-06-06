using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agents_System.BL
{
    public interface IAgentService
    {
        Task<List<Agent>> GetAllAgentsAsync();
        Task<Agent> GetAgentByIdAsync(int id);
        Agent AddAgent(Agent agent);
        Task<Agent> UpdateAgentAsync(Agent agent);
        Task<bool> DeleteAgentAsync(int agentId);
    }
}
