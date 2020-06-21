using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.BL
{
    public interface IPlatformService
    {
        List<Platform> GetAllPlatforms();
        Platform GetPlatformById(int id);
        Platform AddPlatform(Platform platform);
        Platform UpdatePlatform(Platform platform);
        bool DeletePlatform(int platformId);
        List<Platform> GetByPortId(int portId);
        bool CheckUniqueness(Platform platform);
        bool CanDelete(int platformId);
    }
}
