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
            return unitOfWork.Agents.Get().ToList();
        }

        public Agent GetAgentById(int id)
        {
            return GetAllAgents().FirstOrDefault(a => a.AgentId == id);
        }

        public Agent UpdateAgent(Agent agent)
        {
            return unitOfWork.Agents.Update(agent.AgentId, agent);
        }

        public bool CheckUniqueness(Agent agent)
        {
            if (agent.AgentId == 0) //adding
            {
                if (unitOfWork.Agents.Get().FirstOrDefault(a => a.Name == agent.Name) != null)
                    return false;
            }
            else //editing
            {
                var result = unitOfWork.Agents.Get().Where(a => a.Name == agent.Name);
                if (result.Count() > 1)
                    return false;
                else
                if (result.Count() == 1 && result.FirstOrDefault(a => a.AgentId == agent.AgentId) == null)
                    return false;
            }
            return true;
        }

        public bool CanDelete(int agentId)
        {
            return unitOfWork.Trips.Get().FirstOrDefault(t => t.AgentId == agentId) == null;
        }
    }
}
