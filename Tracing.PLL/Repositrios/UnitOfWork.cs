using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracing.DAL.Contexts;
using Tracing.PLL.Interfaces;

namespace Tracing.PLL.Repositrios
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TracingDbContext tracingDbContext;

        public IMovieRepositories movieRepositories { get ; set; }
        public IProducerRepositories producerRepositories { get ; set ; }
        public ICinemaRepositories cinemaRepositories { get ; set ; }
        public IActorRepositories actorRepositories { get ; set ; }

        public UnitOfWork(TracingDbContext tracingDbContext) 
        {
            movieRepositories = new MovieRepositories(tracingDbContext);
            producerRepositories = new ProducerRepositories(tracingDbContext);
            cinemaRepositories = new CinemaRepositories(tracingDbContext);
            actorRepositories = new ActorRepositories(tracingDbContext);
            this.tracingDbContext = tracingDbContext;
        }

        public async Task<int> Complete()
        => await tracingDbContext.SaveChangesAsync();

        public void Dispose()
        {
            tracingDbContext.Dispose();
        }
    }
}
