using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingApi.DTOs.Booking
{
    public record UpdateBookingDto(
        DateOnly CheckIn,
        DateOnly CheckOut,
        [Required][Range(minimum: 1, maximum: 10)] int Guests
    ) : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CheckOut <= CheckIn)
            {
                yield return new ValidationResult(
                    "Check-out date must be after check-in date.",
                    [nameof(CheckOut), nameof(CheckIn)]
                );
            }
        }
    }
}