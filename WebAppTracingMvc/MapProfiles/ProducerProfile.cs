using AutoMapper;
using Tracing.DAL.Entities;
using WebAppTracingMvc.ViewModels;

namespace WebAppTracingMvc.MapProfiles
{
    public class ProducerProfile : Profile
    {
        public ProducerProfile()
        {
            CreateMap<ProducerViewModel,Producer>().ReverseMap();
        }
    }
}
