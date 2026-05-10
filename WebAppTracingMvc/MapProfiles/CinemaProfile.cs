using AutoMapper;
using Tracing.DAL.Entities;
using WebAppTracingMvc.ViewModels;

namespace WebAppTracingMvc.MapProfiles
{
    public class CinemaProfile:Profile
    {
        public CinemaProfile()
        {
            CreateMap<CinemaViewModel, Cinema>().ReverseMap();
        }
    }
}
