using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracing.DAL.Entities;

namespace Tracing.PLL.Interfaces
{
    public interface IGenericRepositories<T>
    {
        public Task<IEnumerable<T>> GetAll();
        public Task<T> Get(int id);
        public Task Create(T item);
        public void Update(T item);
        public void Delete(T item);
    }
}
