using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListingApi.DTOs.Hotel;

namespace HotelListingApi.DTOs.Country
{
    public record GetCountryDto(
        int Id,
        string Name,
        string ShortName,
        List<GetHotelDetailsDto> Hotels
    );


}