using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListingApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace HotelListingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        readonly private static List<Hotel> hotels =
        [
            new Hotel
            {
                Id = 1,
                Name = "Hotel California",
                Address = "42 Sunset Boulevard",
                Rating = 4.5
            },
            new Hotel
            {
                Id = 2,
                Name = "Grand Budapest Hotel",
                Address = "1 Alpine Drive",
                Rating = 4.8
            }
        ];

        [HttpGet("")]
        public ActionResult<IEnumerable<IEnumerable<Hotel>>> GetTModel()
        {
            return Ok(hotels);
        }

        [HttpGet("{id}")]
        public ActionResult<Hotel> GetHotelById(int id)
        {
            var hotel = hotels.FirstOrDefault(h => h.Id == id);
            if (hotel == null)
            {
                return NotFound();
            }
            return Ok(hotel);
        }

        [HttpPost("")]
        public ActionResult<Hotel> PostHotel(Hotel model)
        {
            if (hotels.Any(h => h.Id == model.Id))
            {
                return Conflict("A hotel with the same ID already exists.");
            }
            hotels.Add(model);
            return CreatedAtAction(nameof(GetHotelById), new { id = model.Id }, model);
        }



        [HttpPut("{id}")]
        public IActionResult PutHotel(int id, Hotel model)
        {

            var existingHotel = hotels.FirstOrDefault(h => h.Id == id);
            if (existingHotel == null)
            {
                return NotFound();
            }

            existingHotel.Name = model.Name;
            existingHotel.Address = model.Address;
            existingHotel.Rating = model.Rating;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult<Hotel> DeleteHotelById(int id)
        {
            var hotel = hotels.FirstOrDefault(h => h.Id == id);
            if (hotel == null)
            {
                return NotFound(new { Message = "Hotel not found." });
            }
            hotels.Remove(hotel);
            return Ok(hotel);
        }
    }
}