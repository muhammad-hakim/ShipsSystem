using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.Utilities
{
    public enum ShipTypes
    {
        Ship = 0,
        Sailboat = 1,
        Zaaem =2
    }

    public enum TripStatus
    {
        LeftDGebouti = 0,
        ReservationArea = 1,
        AtGhates = 2,
        ArriveAtPlatform = 3,
        WaitingAtGhatesAfterUnload = 4,
        EXecptedTOArrive =5
    }
    public enum AccidentArea
    {
        InPortArea =0,
        InTerritorialWater =1,
        InInternationalWater = 2,
    }
}
