using AutoMapper;
using System.Linq;
using Tracing.DAL.Entities;
using WebAppTracingMvc.ViewModels;

namespace WebAppTracingMvc.MapProfiles
{
    public class ActorProfile:Profile
    {
        public ActorProfile()
        {
            CreateMap<ActorViewModel, Actor>();
            CreateMap<Actor, ActorViewModel>()
                .ForMember(des=>des.Movies,
                opt=>opt.MapFrom(src=>src.ActorMovies
                .Select(am=>new MovieViewModel
                {
                    Id=am.Movie.Id,
                    Name=am.Movie.Name,
                })));
        }
    }
}
