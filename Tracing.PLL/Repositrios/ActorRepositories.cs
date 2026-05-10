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
    public class ActorRepositories :GenericRepositories<Actor>, IActorRepositories
    {
        private readonly TracingDbContext tracingDbContext;

        public ActorRepositories(TracingDbContext tracingDbContext):base(tracingDbContext) 
        {
            this.tracingDbContext = tracingDbContext;
        }

        public async Task<IEnumerable<Actor>> SearchActorByName(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return await GetAll();

            return await tracingDbContext.Actors
                .Where(m => m.FullName.ToLower().Contains(term.ToLower()))
                .ToListAsync();
        }
    }
}
