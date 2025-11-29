using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingApi.DTOs.Hotel
{
    public class CreateHotelDto
    {
        [Required]
        public required string Name { get; set; }

        [MaxLength(150)]
        public required string Address { get; set; }

        [Range(1, 5)]
        public double Rating { get; set; }

        [Required]
        public int CountryId { get; set; }
    }
}