using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListing.Api.Results;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Country;
using Microsoft.AspNetCore.Mvc;

namespace HotelListingApi.Contracts
{
    public interface ICountriesServices
    {
        Task<bool> CountryExists(int id);
        Task<bool> CountryExists(string name);
        Task<Result> DeleteCountryById(int id);
        Task<Result<IEnumerable<GetCountriesDto>>> GetCountries();
        Task<Result<GetCountryDto>> GetCountryById(int id);
        Task<Result<GetCountryDto>> PostCountry(CreateCountryDto model);
        Task<Result> PutCountry(int id, UpdateCountryDto model);
    }
}