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
    public class CinemaRepositories:GenericRepositories<Cinema>,ICinemaRepositories
    {
        private readonly TracingDbContext tracingDbContext;

        public CinemaRepositories(TracingDbContext tracingDbContext):base(tracingDbContext)
        {
            this.tracingDbContext = tracingDbContext;
        }

        public async Task<IEnumerable<Cinema>> SearchCinemaByName(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return await GetAll();

            return await tracingDbContext.Cinemas
                .Where(m => m.Name.ToLower().Contains(term.ToLower()))
                .ToListAsync();
        }
    }
}
