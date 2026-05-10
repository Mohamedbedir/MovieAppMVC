using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracing.DAL.Contexts;
using Tracing.DAL.Entities;
using Tracing.PLL.Interfaces;

namespace Tracing.PLL.Repositrios
{
    public class GenericRepositories<T> : IGenericRepositories<T> where T : class
    {
        private readonly TracingDbContext tracingDbContext;

        public GenericRepositories(TracingDbContext tracingDbContext)
        {
            this.tracingDbContext = tracingDbContext;
        }
        public async Task Create(T item)
        {
            await tracingDbContext.Set<T>().AddAsync(item);
        }

        public void Delete(T item)
        {
            tracingDbContext.Set<T>().Remove(item);
        }

        public async Task<T> Get(int id)
        {
            if(typeof(T) == (typeof(Movie)))
            {
                var movie = await tracingDbContext.Movies
                         .Include(m => m.Producer)
                         .Include(m => m.Cinema)
                         .Include(m => m.ActorMovies)
                         .ThenInclude(am => am.Actor)
                         .FirstOrDefaultAsync(m => m.Id == id);

                return movie as T;
            }
            else if(typeof(T) == (typeof(Producer)))
            {
                var movie =await tracingDbContext.Producers
                         .Include(p => p.Movies)
                        .FirstOrDefaultAsync(m => m.Id == id);

                return movie as T;
            }
            else if(typeof(T) == (typeof(Cinema)))
            {
                var movie = await tracingDbContext.Cinemas
                         .Include(p => p.Movies)
                        .FirstOrDefaultAsync(m => m.Id == id);

                return movie as T;
            }
            else if (typeof(T) == typeof(Actor))
            {
                var actor = await tracingDbContext.Actors
                    .Include(a => a.ActorMovies)
                    .ThenInclude(am => am.Movie)
                    .FirstOrDefaultAsync(a => a.Id == id);

                return actor as T;
            }

            return await tracingDbContext.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            //if(typeof(T) == (typeof(Movie)))
            //{
            //    return (IEnumerable<T>) tracingDbContext.Movies.Include(p=>p.Producer).ToList();
            //}

           return await tracingDbContext.Set<T>().ToListAsync();
        }

        public void Update(T item)
        {
            tracingDbContext.Set<T>().Update(item);
        }
    }
}
