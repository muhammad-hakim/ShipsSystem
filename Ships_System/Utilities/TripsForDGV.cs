using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.Utilities
{
    public class TripsForDGV
    {
        public string TripId { get; set; }
        public string ShipName { get; set; }
        public string ShipIMO { get; set; }
        public string ShipType { get; set; }
        public string TripStatus { get; set; }
        public string TripStatusDate { get; set; }
        public string Agent { get; set; }
        public string Port { get; set; }
        public string Platform { get; set; }
        public string Notes { get; set; }
        public int TripStatusVal { get; set; }
    }
}
