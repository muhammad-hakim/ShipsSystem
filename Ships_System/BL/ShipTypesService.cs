using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.BL
{
    public class ShipTypesService : IShipTypesService
    {
        private readonly UnitOfWork unitOfWork;

        public ShipTypesService (UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public ShipType AddShipType(ShipType shipType)
        {
            return unitOfWork.ShipTypes.Add(shipType);
        }

        public bool CanDelete(int shipTypeId)
        {
            return unitOfWork.Ships.Get().FirstOrDefault(s => s.Type == shipTypeId) == null;
        }

        public bool CheckUniqueness(ShipType shipType)
        {
            if (shipType.TypeId == 0) //adding
            {
                if (unitOfWork.ShipTypes.Get().FirstOrDefault(t => t.Name == shipType.Name) != null)
                    return false;
            }
            else //editing
            {
                var result = unitOfWork.ShipTypes.Get().Where(t => t.Name == shipType.Name);
                if (result.Count() > 1)
                    return false;
                else
                if (result.Count() == 1 && result.FirstOrDefault(t => t.TypeId == shipType.TypeId) == null)
                    return false;
            }
            return true;
        }

        public bool DeleteShipType(int shipTypeId)
        {
            return unitOfWork.ShipTypes.Delete(shipTypeId);
        }

        public List<ShipType> GetAllShipTypes()
        {
            return unitOfWork.ShipTypes.Get().ToList();
        }

        public ShipType GetShipTypeById(int id)
        {
            return GetAllShipTypes().FirstOrDefault(t => t.TypeId == id);
        }

        public ShipType UpdateShipType(ShipType shipType)
        {
            return unitOfWork.ShipTypes.Update(shipType.TypeId, shipType);
        }
    }
}
