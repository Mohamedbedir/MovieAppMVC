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
    public class ProducerRepositories:GenericRepositories<Producer>,IProducerRepositories
    {
        private readonly TracingDbContext tracingDbContext;

        public ProducerRepositories(TracingDbContext tracingDbContext):base(tracingDbContext)
        {
            this.tracingDbContext = tracingDbContext;
        }

        public async Task<IEnumerable<Producer>> SearchProducerByName(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return await GetAll();

            return await tracingDbContext.Producers
                .Where(m => m.FullName.ToLower().Contains(term.ToLower()))
                .ToListAsync();
        }
    }
}
