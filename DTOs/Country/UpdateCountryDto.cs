using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingApi.DTOs.Country
{
    public class UpdateCountryDto : CreateCountryDto
    {
        [Required]
        public int Id { get; set; }
    }
}