using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.BL
{
    public interface IAgentService
    {
        List<Agent> GetAllAgents();
        Agent GetAgentById(int id);
        Agent AddAgent(Agent agent);
        Agent UpdateAgent(Agent agent);
        bool DeleteAgent(int agentId);
    }
}
