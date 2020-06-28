using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.Utilities
{
    public enum TripStatus
    {
        LeftDGebouti = 0,
        ReservationArea = 1,
        EXecptedTOArrive = 2,
        AtGhates = 3,
        ArriveAtPlatform = 4,
        WaitingAtGhatesAfterUnload = 5
    }
    public enum AccidentArea
    {
        InPortArea =0,
        InTerritorialWater =1,
        InInternationalWater = 2,
    }
}
