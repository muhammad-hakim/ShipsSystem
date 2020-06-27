using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ships_System.DAL;

namespace Ships_System.BL
{
    public class ShipService : IShipService
    {
        private readonly UnitOfWork unitOfWork;

        public ShipService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Ship AddShip(Ship ship)
        {
            return unitOfWork.Ships.Add(ship);
        }

        public bool CanDelete(int shipId)
        {
            return unitOfWork.Trips.Get().FirstOrDefault(t => t.ShipId == shipId) == null && 
                unitOfWork.Accidents.Get().FirstOrDefault(a => a.ShipId == shipId) == null; 
        }

        public bool CheckUniqueness(Ship ship)
        {
            if (ship.ShipId == 0) //adding
            {
                if (unitOfWork.Ships.Get().FirstOrDefault(s => s.Name == ship.Name || s.Imo == ship.Imo) != null)
                    return false;
            }
            else //editing
            {
                var result = unitOfWork.Ships.Get().Where(s => s.Name == ship.Name || s.Imo == ship.Imo);
                    if (result.Count() > 1)
                    return false;
                else
                    if (result.Count() == 1 && result.FirstOrDefault(s => s.ShipId == ship.ShipId) == null)
                    return false;
            }
            return true;
        }

        public bool DeleteShip(int shipId)
        {
            return  unitOfWork.Ships.Delete(shipId);
        }

        public List<Ship> GetAllShips()
        {
            return unitOfWork.Ships.Get().Include(t => t.ShipType).ToList();
        }

        public Ship GetShipById(int id)
        {
            return  GetAllShips().FirstOrDefault(s => s.ShipId == id);
        }

        public Ship UpdateShip(Ship ship)
        {
            return  unitOfWork.Ships.Update(ship.ShipId, ship);
        }
    }
}
