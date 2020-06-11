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
        List<Port> GetAllPorts();
        Port GetPortById(int id);
        Port AddPort(Port port);
        Port UpdatePort(Port port);
        bool DeletePort(int portId);
    }
}
