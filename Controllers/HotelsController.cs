using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListingApi.Contracts;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Hotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HotelsController(IHotelsService hotelsService) : ControllerBase
{

    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<GetHotelDto>>> GetHotels()
    {
        var hotels = await hotelsService.GetHotels();

        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetHotelDto>> GetHotelById(int id)
    {
        var hotel = await hotelsService.GetHotelById(id);

        if (hotel == null)
        {
            return NotFound();
        }

        return Ok(hotel);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> PutHotel(int id, UpdateHotelDto model)
    {
        if (id != model.Id)
        {
            return BadRequest("ID mismatch.");
        }

        await hotelsService.PutHotel(id, model);

        return NoContent();
    }

    [HttpPost("")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDto model)
    {

        var hotel = await hotelsService.PostHotel(model);

        return CreatedAtAction(nameof(GetHotelById), new { id = hotel.Id }, hotel);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        await hotelsService.DeleteHotelById(id);

        return NoContent();
    }
}