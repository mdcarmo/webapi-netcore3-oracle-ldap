using AutoMapper;
using SmallApi.Domain.Entity;
using SmallApi.Dto;

namespace SmallApi.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Person, PersonDTO>().ReverseMap();
        }
    }
}
