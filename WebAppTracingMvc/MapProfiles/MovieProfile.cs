using AutoMapper;
using System.Linq;
using Tracing.DAL.Entities;
using WebAppTracingMvc.ViewModels;

namespace WebAppTracingMvc.MapProfiles
{
    public class MovieProfile:Profile
    {
        public MovieProfile()
        {
            CreateMap<MovieViewModel, Movie>();
            CreateMap<Movie, MovieViewModel>()
             .ForMember(dest => dest.Actors,
             opt => opt.MapFrom(src => src.ActorMovies
            .Select(am => new ActorViewModel
            {
                Id = am.Actor.Id,
                FullName = am.Actor.FullName
            })));
        }
    }
}
