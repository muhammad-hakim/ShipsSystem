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

        public bool DeleteTrip(int TripId)
        {
            return unitOfWork.Trips.Delete(TripId);
        }

        public List<Trip> GetAllTrips()
        {
            return unitOfWork.Trips.Get();
        }

        public Trip GetTripById(int id)
        {
            return unitOfWork.Trips.GetById(id);
        }

        public Trip UpdateTrip(Trip Trip)
        {
            return unitOfWork.Trips.Update(Trip.TripId, Trip);
        }
    }
}
