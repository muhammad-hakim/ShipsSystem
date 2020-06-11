using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.DAL
{
    public interface IRepository<T> where T : class
    {
        List<T> Get();
        T GetById(int id);
        T Add(T obj);
        T Update(int id, T obj);
        bool Delete(int id);
    }
}
