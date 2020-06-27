using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
            try
            {
                data.Add(obj);
                return obj;
            } catch (Exception ex)
            {
                return null;
            }
        }

        public bool Delete(int id)
        {
            var target = data.Find(id);
            if (target != null)
            {
                data.Remove(target);
                return true;
            }
            return false;
        }

        public IQueryable<T> Get()
        {
            var result =  data; ;
            return result;
        }

        public T GetById(int id)
        {
            return data.Find(id);
        }

        public T Update(int id, T obj)
        {
            data.AddOrUpdate(obj);
            return obj;
        }
    }
}
