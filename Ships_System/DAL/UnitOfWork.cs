using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.DAL
{
    public class UnitOfWork
    {
        private readonly SystemContext context;

        public UnitOfWork(SystemContext context)
        {
            this.context = context;
        }

        public Repository<Agent> Agents
        {
            get
            {
                return new Repository<Agent>(context);
            }
        }

        public Repository<Platform> Platforms
        {
            get
            {
                return new Repository<Platform>(context);
            }
        }

        public Repository<Port> Ports
        {
            get
            {
                return new Repository<Port>(context);
            }
        }

        public Repository<Product> Products
        {
            get
            {
                return new Repository<Product>(context);
            }
        }

        public Repository<Ship> Ships
        {
            get
            {
                return new Repository<Ship>(context);
            }
        }

        public Repository<Trip> Trips
        {
            get
            {
                return new Repository<Trip>(context);
            }
        }

        public Repository<TripsLoad> TripsLoads
        {
            get
            {
                return new Repository<TripsLoad>(context);
            }
        }

        public Repository<TripsStatu> TripsStatus
        {
            get
            {
                return new Repository<TripsStatu>(context);
            }
        }

        public bool Commit()
        {
            return context.SaveChanges() > 0;
        }
    }
}
