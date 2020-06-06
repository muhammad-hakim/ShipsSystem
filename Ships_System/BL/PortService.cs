using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<bool> DeletePortAsync(int portId)
        {
            return await unitOfWork.Ports.DeleteAsync(portId);
        }

        public async Task<List<Port>> GetAllPortsAsync()
        {
            return await unitOfWork.Ports.GetAsync();
        }

        public async Task<Port> GetPortByIdAsync(int id)
        {
            return await unitOfWork.Ports.GetByIdAsync(id);
        }

        public async Task<Port> UpdatePortAsync(Port port)
        {
            return await unitOfWork.Ports.UpdateAsync(port.PortId, port);
        }

    }
}
