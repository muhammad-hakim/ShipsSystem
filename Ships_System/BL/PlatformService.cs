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

        public bool CanDelete(int platformId)
        {
            return unitOfWork.Trips.Get().FirstOrDefault(t => t.PlatformId == platformId) == null;
        }

        public bool CheckUniqueness(Platform platform)
        {
            if (platform.PlatformId == 0) //adding
            {
                if (unitOfWork.Platforms.Get().FirstOrDefault(a => a.Name == platform.Name && a.PortId == platform.PortId) != null)
                    return false;
            }
            else //editing
            {
                var result = unitOfWork.Platforms.Get().Where(a => a.Name == platform.Name && a.PortId == platform.PortId);
                if (result.Count() > 1)
                    return false;
                else
                if (result.Count() == 1 && result.FirstOrDefault(a => a.PlatformId == platform.PlatformId) == null)
                    return false;
            }
            return true;
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
            return  GetAllPlatforms().FirstOrDefault(p => p.PlatformId == id);
        }

        public Platform UpdatePlatform(Platform platform)
        {
            return  unitOfWork.Platforms.Update(platform.PlatformId, platform);
        }        
    }
}
