using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.BL
{
    public class DbService : IDbService
    {
        private readonly UnitOfWork unitOfWork;

        public DbService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public bool Commit()
        {
            return unitOfWork.Commit();
        }
    }
}
