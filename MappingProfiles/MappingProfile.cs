using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Country;
using HotelListingApi.DTOs.Hotel;

namespace HotelListingApi.MappingProfiles
{
    public class HotelMappingProfile : Profile
    {
        public HotelMappingProfile()
        {
            CreateMap<Hotel, GetHotelDto>()
                .ForMember(dest => dest.Country, opt => opt.MapFrom<CountryNameResolver>());

            CreateMap<CreateHotelDto, Hotel>();
            CreateMap<UpdateHotelDto, Hotel>();
        }
    }

    public class CountryMappingProfile : Profile
    {
        public CountryMappingProfile()
        {
            CreateMap<Country, GetCountryDto>();
            CreateMap<Country, GetCountriesDto>();
            CreateMap<CreateCountryDto, Country>();
        }
    }

    public class CountryNameResolver : IValueResolver<Hotel, GetHotelDto, string>
    {
        public string Resolve(Hotel source, GetHotelDto destination, string destMember, ResolutionContext context)
        {
            return source.Country != null ? source.Country.Name : string.Empty;
        }
    }
}