using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ships_System.DAL;

namespace Ships_System.BL
{
    public class AccidentService : IAccidentService
    {
        private readonly UnitOfWork unitOfWork;
        public AccidentService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Accident AddAccident(Accident accident)
        {
            return unitOfWork.Accidents.Add(accident);
        }

        public bool DeleteAccident(int accidentId)
        {
            return unitOfWork.Accidents.Delete(accidentId);
        }

        public List<Accident> GetAllAccidents()
        {
            return unitOfWork.Accidents.Get().Include("Ship").ToList();
        }

        public Accident GetAccidentById(int id)
        {
            return GetAllAccidents().FirstOrDefault(a => a.AccidentId == id);

        }

        public Accident UpdateAccident(Accident accident)
        {
            return unitOfWork.Accidents.Update(accident.AccidentId, accident);
        }
    }
}
