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

        public bool DeletePort(int portId)
        {
            return  unitOfWork.Ports.Delete(portId);
        }

        public List<Port> GetAllPorts()
        {
            return  unitOfWork.Ports.Get();
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
