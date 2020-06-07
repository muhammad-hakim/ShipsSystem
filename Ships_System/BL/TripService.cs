using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.BL
{
    public class TripService : ITripService
    {
        private readonly UnitOfWork unitOfWork;

        public TripService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Trip AddTrip(Trip trip)
        {
            return unitOfWork.Trips.Add(trip);
        }

        public async Task<bool> DeleteTripAsync(int TripId)
        {
            return await unitOfWork.Trips.DeleteAsync(TripId);
        }

        public async Task<List<Trip>> GetAllTripsAsync()
        {
            return await unitOfWork.Trips.GetAsync();
        }

        public async Task<Trip> GetTripByIdAsync(int id)
        {
            return await unitOfWork.Trips.GetByIdAsync(id);
        }

        public async Task<Trip> UpdateTripAsync(Trip Trip)
        {
            return await unitOfWork.Trips.UpdateAsync(Trip.TripId, Trip);
        }

    }
}
