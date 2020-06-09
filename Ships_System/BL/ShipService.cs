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

        public async Task<bool> DeleteShipAsync(int shipId)
        {
            return await unitOfWork.Ships.DeleteAsync(shipId);
        }

        public async Task<List<Ship>> GetAllShipsAsync()
        {
            return await unitOfWork.Ships.GetAsync();
        }

        public async Task<Ship> GetShipByIdAsync(int id)
        {
            return await unitOfWork.Ships.GetByIdAsync(id);
        }

        public async Task<Ship> UpdateShipAsync(Ship ship)
        {
            return await unitOfWork.Ships.UpdateAsync(ship.ShipId, ship);
        }
    }
}
