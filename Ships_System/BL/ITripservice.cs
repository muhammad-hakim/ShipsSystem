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
        Task<List<Trip>> GetAllTripsAsync();
        Task<Trip> GetTripByIdAsync(int id);
        Trip AddTrip(Trip trip);
        Task<Trip> UpdateTripAsync(Trip trip);
        Task<bool> DeleteTripAsync(int tripId);
    }
}
