using AutoMapper;
using $safeprojectname$.Domain.Entity;
using $safeprojectname$.Dto;

namespace $safeprojectname$.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Person, PersonDTO>().ReverseMap();
        }
    }
}
