using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.BL
{
    public class PlatformService : IPlatformService
    {
        private readonly UnitOfWork unitOfWork;

        public PlatformService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Platform AddPlatform(Platform platform)
        {
            return unitOfWork.Platforms.Add(platform);
        }

        public async Task<bool> DeletePlatformAsync(int platformId)
        {
            return await unitOfWork.Platforms.DeleteAsync(platformId);
        }

        public async Task<List<Platform>> GetAllPlatformsAsync()
        {
            return await unitOfWork.Platforms.GetAsync();
        }

        public async Task<Platform> GetPlatformByIdAsync(int id)
        {
            return await unitOfWork.Platforms.GetByIdAsync(id);
        }

        public async Task<Platform> UpdatePlatformAsync(Platform platform)
        {
            return await unitOfWork.Platforms.UpdateAsync(platform.PlatformId, platform);
        }

    }
}
