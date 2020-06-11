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
        List<Ship> GetAllShips();
        Ship GetShipById(int id);
        Ship AddShip(Ship ship);
        Ship UpdateShip(Ship ship);
        bool DeleteShip(int shipId);
    }
}
