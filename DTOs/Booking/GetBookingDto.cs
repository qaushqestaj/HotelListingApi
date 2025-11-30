using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingApi.DTOs.Booking
{
    public record GetBookingDto(
        int Id,
        int HotelId,
        string HotelName,
        DateOnly CheckIn,
        DateOnly CheckOut,
        int Guests,
        decimal TotalPrice,
        string Status,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );
}