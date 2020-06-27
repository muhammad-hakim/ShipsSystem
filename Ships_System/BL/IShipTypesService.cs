using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.BL
{
    public interface IShipTypesService
    {
        List<ShipType> GetAllShipTypes();
        ShipType GetShipTypeById(int id);
        ShipType AddShipType(ShipType shipType);
        ShipType UpdateShipType(ShipType shipType);
        bool DeleteShipType(int shipTypeId);
        bool CheckUniqueness(ShipType shipType);
        bool CanDelete(int shipTypeId);
    }
}
