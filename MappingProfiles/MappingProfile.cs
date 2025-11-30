using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Booking;
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

    public sealed class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            CreateMap<Booking, GetBookingDto>()
            .ForMember(
                dest => dest.HotelName,
                opt => opt.MapFrom(src => src.Hotel!.Name))
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString())
            );

            CreateMap<CreateBookingDto, Booking>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.Hotel, opt => opt.Ignore());

            CreateMap<UpdateBookingDto, Booking>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.Hotel, opt => opt.Ignore());
        }
    }
}