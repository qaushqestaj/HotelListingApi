using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListing.Api.Results;
using HotelListingApi.Contracts;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Country;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CountriesController(ICountriesServices countriesServices) : BaseApiController
{
    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<GetCountriesDto>>> GetCountries()
    {
        var result = await countriesServices.GetCountries();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCountryDto>> GetCountryById(int id)
    {
        var country = await countriesServices.GetCountryById(id);

        return Ok(country);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto model)
    {
        if (id != model.Id)
        {
            return BadRequest("ID mismatch.");
        }

        await countriesServices.PutCountry(id, model);

        return NoContent();
    }

    [HttpPost("")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<GetCountryDto>> PostCountry(CreateCountryDto createDto)
    {
        var result = await countriesServices.PostCountry(createDto);
        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

        return CreatedAtAction(nameof(GetCountryById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<Country>> DeleteCountryById(int id)
    {
        await countriesServices.DeleteCountryById(id);
        return NoContent();
    }

}