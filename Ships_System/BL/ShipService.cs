using System;
using System.Collections.Generic;
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

        public bool DeleteShip(int shipId)
        {
            return  unitOfWork.Ships.Delete(shipId);
        }

        public List<Ship> GetAllShips()
        {
            return  unitOfWork.Ships.Get();
        }

        public Ship GetShipById(int id)
        {
            return  unitOfWork.Ships.GetById(id);
        }

        public Ship UpdateShip(Ship ship)
        {
            return  unitOfWork.Ships.Update(ship.ShipId, ship);
        }
    }
}
