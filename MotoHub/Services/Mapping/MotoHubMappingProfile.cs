
using AutoMapper;
using MotoHub.DAL.Entities;
using MotoHub.DTOs;

namespace MotoHub.Services.Mapping
{
    public class MotoHubMappingProfile : Profile
    {
        public MotoHubMappingProfile()
        {
            CreateMap<Motorcycle, MotorcycleDto>()
                .ForMember(
                    d => d.BrandName,
                    o => o.MapFrom(s => s.Brand.Name))
                .ForMember(
                    d => d.CategoryName,
                    o => o.MapFrom(s => s.Category.Name));

            CreateMap<CreateMotorcycleDto, Motorcycle>();

            CreateMap<UpdateMotorcycleDto, Motorcycle>();

            CreateMap<Brand, BrandDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Service, ServiceDto>();
        }
    }
}
