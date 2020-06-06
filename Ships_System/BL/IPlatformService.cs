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
        Task<List<Platform>> GetAllPlatformsAsync();
        Task<Platform> GetPlatformByIdAsync(int id);
        Platform AddPlatform(Platform platform);
        Task<Platform> UpdatePlatformAsync(Platform platform);
        Task<bool> DeletePlatformAsync(int platformId);
    }
}
