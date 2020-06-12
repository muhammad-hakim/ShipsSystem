using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public bool DeletePlatform(int platformId)
        {
            return  unitOfWork.Platforms.Delete(platformId);
        }

        public List<Platform> GetAllPlatforms()
        {
            return  unitOfWork.Platforms.Get().AsQueryable().Include("Port").ToList();
        }

        public List<Platform> GetByPortId(int portId)
        {
            return unitOfWork.Platforms.Get().Where(p => p.PortId == portId).ToList();
        }

        public Platform GetPlatformById(int id)
        {
            return  unitOfWork.Platforms.GetById(id);
        }

        public Platform UpdatePlatform(Platform platform)
        {
            return  unitOfWork.Platforms.Update(platform.PlatformId, platform);
        }        
    }
}
