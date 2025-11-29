using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingApi.DTOs.Country
{
    public record GetCountriesDto(
        int Id,
        string Name,
        string ShortName
    );
}