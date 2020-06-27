using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public bool DeleteTrip(int tripId)
        {
            Trip trip = GetTripById(tripId);

            var statusIdsToRemove = trip.TripsStatus.Select(x => x.Id).ToList();
            var loadsIdsToRemove = trip.TripsLoads.Select(x => x.Id).ToList();

            foreach (var id in statusIdsToRemove)
            {
                unitOfWork.TripsStatus.Delete(id);
            }

            foreach (var id in loadsIdsToRemove)
            {
                unitOfWork.TripsLoads.Delete(id);
            }

            return unitOfWork.Trips.Delete(tripId);
        }

        public List<Trip> GetAllTrips()
        {
            return unitOfWork.Trips.Get().Include("Agent.Ship.Port.Platform.TripsLoads.TripsStatus").ToList();
        }

        public Trip GetTripById(int tripId)
        {
            return GetAllTrips().FirstOrDefault(t => t.TripId == tripId);
        }

        public Trip UpdateTrip(Trip trip)
        {
            return unitOfWork.Trips.Update(trip.TripId, trip);
        }
    }
}
