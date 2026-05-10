using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracing.DAL.Entities;

namespace Tracing.PLL.Interfaces
{
    public interface ICinemaRepositories: IGenericRepositories<Cinema>
    {
        Task<IEnumerable<Cinema>> SearchCinemaByName(string term);

    }
}
