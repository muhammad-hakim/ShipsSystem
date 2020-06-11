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

        public bool DeleteAgent(int agentId)
        {
            return unitOfWork.Agents.Delete(agentId);
        }

        public List<Agent> GetAllAgents()
        {
            return unitOfWork.Agents.Get();
        }

        public Agent GetAgentById(int id)
        {
            return unitOfWork.Agents.GetById(id);
        }

        public Agent UpdateAgent(Agent agent)
        {
            return unitOfWork.Agents.Update(agent.AgentId, agent);
        }
    }
}
