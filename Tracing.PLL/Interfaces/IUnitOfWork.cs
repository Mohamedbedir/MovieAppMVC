using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.PLL.Interfaces
{
    public interface IUnitOfWork :IDisposable
    {
        // collect all repos
        IMovieRepositories movieRepositories { get; set; }
        IProducerRepositories producerRepositories { get; set; }
        ICinemaRepositories cinemaRepositories { get; set; }
        IActorRepositories actorRepositories { get; set; }

        Task<int> Complete();

    }
}
