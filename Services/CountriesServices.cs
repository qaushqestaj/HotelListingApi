using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListing.Api.Results;
using HotelListingApi.Contracts;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Country;
using HotelListingApi.DTOs.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Services;

public class CountriesServices(HotelListingDbContext context) : ICountriesServices
{
    public async Task<Result<IEnumerable<GetCountriesDto>>> GetCountries()
    {
        var countries = await context.Countries
                .Select(c => new GetCountriesDto(c.Id, c.Name, c.ShortName)).ToListAsync();

        return Result<IEnumerable<GetCountriesDto>>.Success(countries);
    }

    public async Task<Result<GetCountryDto>> GetCountryById(int id)
    {
        var country = await context.Countries
            .Where(c => c.Id == id)
            .Select(c => new GetCountryDto(c.Id, c.Name, c.ShortName, c.Hotels
                .Select(h => new GetHotelDetailsDto(h.Id, h.Name, h.Address, h.Rating)).ToList()))
            .FirstOrDefaultAsync();

        return country is null
            ? Result<GetCountryDto>.Failure()
            : Result<GetCountryDto>.Success(country);
    }



    public async Task<Result> PutCountry(int id, UpdateCountryDto model)
    {
        try
        {
            if (id != model.Id)
            {
                return Result.BadRequest(new Error("Validation", "The provided ID does not match the country ID."));
            }

            var country = await context.Countries.FindAsync(id);
            if (country is null)
            {
                return Result.NotFound(new Error("NotFound", "Country not found."));
            }

            country.Name = model.Name;
            country.ShortName = model.ShortName;

            context.Update(country);
            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("DatabaseError", ex.Message));
        }
    }


    public async Task<Result<GetCountryDto>> PostCountry(CreateCountryDto model)
    {
        try
        {
            var exists = await CountryExists(model.Name);
            if (exists)
            {
                return Result<GetCountryDto>.Failure(new Error("DuplicateCountry", "A country with the same name already exists."));
            }

            var country = new Country
            {
                Name = model.Name,
                ShortName = model.ShortName
            };

            context.Countries.Add(country);
            await context.SaveChangesAsync();

            var dto = new GetCountryDto(country.Id, country.Name, country.ShortName, []);

            return Result<GetCountryDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<GetCountryDto>.Failure(new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result> DeleteCountryById(int id)
    {
        try
        {

            var country = await context.Countries.FindAsync(id);
            if (country is null)
            {
                return Result.NotFound(new Error("NotFound", "Country not found."));
            }

            context.Countries.Remove(country);
            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<bool> CountryExists(int id)
    {
        return await context.Countries.AnyAsync(e => e.Id == id);
    }

    public async Task<bool> CountryExists(string name)
    {
        return await context.Countries.AnyAsync(e => e.Name.ToLower().Trim() == name.ToLower().Trim());
    }
}