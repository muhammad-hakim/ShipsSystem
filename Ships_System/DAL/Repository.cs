using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.DAL
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly SystemContext context;
        private readonly DbSet<T> data;

        public Repository(SystemContext context)
        {
            this.context = context;
            this.data = this.context.Set<T>();
        }

        public T Add(T obj)
        {
            var result = data.Add(obj);
            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var target = await data.FindAsync(id);
            if (target != null)
            {
                data.Remove(target);
                return true;
            }
            return false;
        }

        public async Task<List<T>> GetAsync()
        {
            var result =  await data.ToListAsync(); ;
            return result;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await data.FindAsync(id);
        }

        public async Task<T> UpdateAsync(int id, T obj)
        {
            var target = await data.FindAsync(id);

            if (target != null)
            {
                target = obj;
                return obj;
            }
            return null;
        }
    }
}
