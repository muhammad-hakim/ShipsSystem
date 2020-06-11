using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.BL
{
   public interface ITripService
    {
        List<Trip> GetAllTrips();
        Trip GetTripById(int id);
        Trip AddTrip(Trip trip);
        Trip UpdateTrip(Trip trip);
        bool DeleteTrip(int tripId);
    }
}
