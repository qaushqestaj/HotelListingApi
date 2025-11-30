using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListing.Api.Results;
using HotelListingApi.DTOs.Booking;

namespace HotelListingApi.Contracts
{
    public interface IBookingService
    {
        Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto createBookingDto);
        Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto);
        Task<Result> CancelBookingAsync(int hotelId, int bookingId);
        Task<Result> AdminCancelBookingAsync(int hotelId, int bookingId);
        Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId);
        Task<Result<IEnumerable<GetBookingDto>>> GetUserBookingsForHotelAsync(int hotelId);
        Task<Result<IEnumerable<GetBookingDto>>> GetAdminBookingsForHotelAsync(int hotelId);
    }
}