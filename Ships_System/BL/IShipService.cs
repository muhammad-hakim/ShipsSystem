using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.BL
{
    public interface IShipService
    {
        Task<List<Ship>> GetAllShipsAsync();
        Task<Ship> GetShipByIdAsync(int id);
        Ship AddShip(Ship ship);
        Task<Ship> UpdateShipAsync(Ship ship);
        Task<bool> DeleteShipAsync(int shipId);
    }
}
