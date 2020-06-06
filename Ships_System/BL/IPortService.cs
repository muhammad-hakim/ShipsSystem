using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.BL
{
    public interface IPortService
    {
        Task<List<Port>> GetAllPortsAsync();
        Task<Port> GetPortByIdAsync(int id);
        Port AddPort(Port port);
        Task<Port> UpdatePortAsync(Port port);
        Task<bool> DeletePortAsync(int portId);
    }
}
