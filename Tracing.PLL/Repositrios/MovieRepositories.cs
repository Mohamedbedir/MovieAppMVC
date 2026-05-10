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
    public class MovieRepositories :GenericRepositories<Movie>, IMovieRepositories
    {
        private readonly TracingDbContext tracingDbContext;

        public MovieRepositories(TracingDbContext tracingDbContext) :base(tracingDbContext)
        {
            this.tracingDbContext = tracingDbContext;
        }

        public async Task<IEnumerable<Movie>> SearchMovieByName(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return await GetAll();

            return await tracingDbContext.Movies
                .Where(m => m.Name.ToLower().Contains(term.ToLower()))
                .ToListAsync();
        }
    }
}
