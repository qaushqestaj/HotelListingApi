using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingApi.DTOs.Hotel
{
    public record GetHotelDto(
        int Id,
        string Name,
        string Address,
        double Rating,
        int CountryId,
        string Country
    );

    public record GetHotelDetailsDto(
       int Id,
       string Name,
       string Address,
       double Rating
   );
}