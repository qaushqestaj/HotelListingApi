using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingApi.DTOs.Auth
{
    public class RegisterUserDto : IValidatableObject
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        public int? AssociatedHotelId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Role == "Hotel Admin" && AssociatedHotelId.GetValueOrDefault() < 1)
            {
                yield return new ValidationResult(
                    "AssociatedHotelId is required when Role is 'Hotel Admin'.",
                    [nameof(AssociatedHotelId)]);
            }
        }
    }
}