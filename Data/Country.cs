using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingApi.Data
{
    public class Country
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string ShortName { get; set; }
        public List<Hotel> Hotels { get; set; } = [];
    }
}