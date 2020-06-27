using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ships_System.BL
{
    public class PortService : IPortService
    {
        private readonly UnitOfWork unitOfWork;

        public PortService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Port AddPort(Port port)
        {
            return unitOfWork.Ports.Add(port);
        }

        public bool CanDelete(int portId)
        {
            return unitOfWork.Trips.Get().FirstOrDefault(t => t.ShipId == portId) == null &&
                unitOfWork.Platforms.Get().FirstOrDefault(p => p.PortId == portId) == null;
        }

        public bool CheckUniqueness(Port port)
        {
            if (port.PortId == 0) //adding
            {
                if (unitOfWork.Ports.Get().FirstOrDefault(a => a.Name == port.Name) != null)
                    return false;
            }
            else //editing
            {
                var result = unitOfWork.Ports.Get().Where(a => a.Name == port.Name);
                if (result.Count() > 1)
                    return false;
                else
                if (result.Count() == 1 && result.FirstOrDefault(a => a.PortId == port.PortId) == null)
                    return false;
            }
            return true;
        }

        public bool DeletePort(int portId)
        {
            return  unitOfWork.Ports.Delete(portId);
        }

        public List<Port> GetAllPorts()
        {
            return  unitOfWork.Ports.Get().ToList();
        }

        public Port GetPortById(int id)
        {
            return  unitOfWork.Ports.GetById(id);
        }

        public Port UpdatePort(Port port)
        {
            return unitOfWork.Ports.Update(port.PortId, port);
        }
    }
}
