using System;
using Ships_System.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.BL
{
    public interface IAccidentService
    {
        List<Accident> GetAllAccidents();
        Accident  GetAccidentById(int id);
        Accident AddAccident(Accident accident);
        Accident UpdateAccident(Accident accident);
         bool DeleteAccident(int accidentId);
    }
}
