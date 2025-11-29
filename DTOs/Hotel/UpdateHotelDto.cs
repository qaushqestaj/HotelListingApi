using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingApi.DTOs.Hotel
{
    public class UpdateHotelDto : CreateHotelDto
    {
        [Required]
        public int Id { get; set; }
    }
}