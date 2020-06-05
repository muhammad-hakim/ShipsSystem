using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.DAL
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAsync();
        Task<T> GetByIdAsync(int id);
        T Add(T obj);
        Task<T> UpdateAsync(int id, T obj);
        Task<bool> DeleteAsync(int id);
    }
}
