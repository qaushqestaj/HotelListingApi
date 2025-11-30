using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Results;
using HotelListingApi.Common.Constans;
using HotelListingApi.Contracts;
using HotelListingApi.Data;
using HotelListingApi.Data.Enums;
using HotelListingApi.DTOs.Booking;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Services
{
    public class BookingService(HotelListingDbContext context, IUsersService usersService, IMapper mapper) : IBookingService
    {
        public async Task<Result<IEnumerable<GetBookingDto>>> GetUserBookingsForHotelAsync(int hotelId)
        {
            // Implementation for retrieving bookings for a specific hotel

            var userId = usersService.GetUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Result<IEnumerable<GetBookingDto>>.Failure(new Error(ErrorCodes.Validation, "User not authenticated."));
            }

            var hotelExists = await context.Hotels.AnyAsync(h => h.Id == hotelId);
            if (!hotelExists)
            {
                return Result<IEnumerable<GetBookingDto>>.Failure(new Error(ErrorCodes.NotFound, "Hotel not found."));
            }

            var bookings = await context.Bookings
                .Where(b => b.HotelId == hotelId && b.UserId == userId)
                .OrderBy(b => b.CheckIn)
                .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
                // .Select(b => new GetBookingDto(
                //     b.Id,
                //     b.HotelId,
                //     b.Hotel!.Name,
                //     b.CheckIn,
                //     b.CheckOut,
                //     b.Guests,
                //     b.TotalPrice,
                //     b.Status.ToString(),
                //     b.CreatedAtUtc,
                //     b.UpdatedAtUtc
                // ))
                .ToListAsync();

            return Result<IEnumerable<GetBookingDto>>.Success(bookings);

        }

        public async Task<Result<IEnumerable<GetBookingDto>>> GetAdminBookingsForHotelAsync(int hotelId)
        {
            // Implementation for retrieving bookings for a specific hotel
            var userId = usersService.GetUserId;

            var isHotelAdminUser = await context.HotelAdmins.AnyAsync(ha => ha.HotelId == hotelId && ha.UserId == userId);

            var hotelExists = await context.Hotels.AnyAsync(h => h.Id == hotelId);
            if (!hotelExists)
            {
                return Result<IEnumerable<GetBookingDto>>.Failure(new Error(ErrorCodes.NotFound, "Hotel not found."));
            }

            var bookings = await context.Bookings
                .Where(b => b.HotelId == hotelId)
                .OrderBy(b => b.CheckIn)
                .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
                // .Select(b => new GetBookingDto(
                //     b.Id,
                //     b.HotelId,
                //     b.Hotel!.Name,
                //     b.CheckIn,
                //     b.CheckOut,
                //     b.Guests,
                //     b.TotalPrice,
                //     b.Status.ToString(),
                //     b.CreatedAtUtc,
                //     b.UpdatedAtUtc
                // ))
                .ToListAsync();

            return Result<IEnumerable<GetBookingDto>>.Success(bookings);

        }



        public async Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto createBookingDto)
        {
            // Implementation for creating a new booking for a specific hotel
            var userId = usersService.GetUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "User not authenticated."));
            }


            bool overlaps = await context.Bookings.AnyAsync(b =>
              b.HotelId == createBookingDto.HotelId &&
              b.CheckIn < createBookingDto.CheckOut &&
              b.CheckOut > createBookingDto.CheckIn &&
              b.Status != BookingStatus.Cancelled &&
              b.UserId == userId
          );

            if (overlaps)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict, "Booking dates overlap with an existing booking."));
            }

            var hotel = await context.Hotels.FindAsync(createBookingDto.HotelId);
            if (hotel is null)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, "Hotel not found."));
            }


            var nights = createBookingDto.CheckOut.DayNumber - createBookingDto.CheckIn.DayNumber;
            var totalPrice = hotel.PerNightRate * nights;

            var booking = mapper.Map<Booking>(createBookingDto);
            booking.UserId = userId;

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            var getBookingDto = mapper.Map<GetBookingDto>(booking);

            return Result<GetBookingDto>.Success(getBookingDto);
        }

        public async Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto)
        {

            var userId = usersService.GetUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "User not authenticated."));
            }


            bool overlaps = await context.Bookings.AnyAsync(b =>
                b.HotelId == hotelId &&
                b.Id != bookingId &&
                b.CheckIn < updateBookingDto.CheckOut &&
                b.CheckOut > updateBookingDto.CheckIn &&
                b.Status != BookingStatus.Cancelled &&
                b.UserId == userId
            );

            if (overlaps)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict, "Booking dates overlap with an existing booking."));
            }

            var booking = await context.Bookings
            .Include(b => b.Hotel)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId && b.UserId == userId);

            if (booking == null)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, "Booking not found."));
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "Cannot update a cancelled booking."));
            }

            mapper.Map(updateBookingDto, booking);
            var perNight = booking.Hotel!.PerNightRate;
            var nights = updateBookingDto.CheckOut.DayNumber - updateBookingDto.CheckIn.DayNumber;
            booking.TotalPrice = perNight * nights;
            booking.UpdatedAtUtc = DateTime.UtcNow;

            await context.SaveChangesAsync();

            var getBookingDto = mapper.Map<GetBookingDto>(booking);

            return Result<GetBookingDto>.Success(getBookingDto);
        }

        public async Task<Result> CancelBookingAsync(int hotelId, int bookingId)
        {
            var userId = usersService.GetUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Result.Failure(new Error(ErrorCodes.Validation, "User not authenticated."));
            }

            var booking = await context.Bookings
                .Include(b => b.Hotel)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId && b.UserId == userId);

            if (booking == null)
            {
                return Result.Failure(new Error(ErrorCodes.NotFound, "Booking not found."));
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                return Result.Failure(new Error(ErrorCodes.Validation, "Booking is already cancelled."));
            }

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAtUtc = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> AdminCancelBookingAsync(int hotelId, int bookingId)
        {
            var userId = usersService.GetUserId;

            var isHotelAdminUser = await context.HotelAdmins.AnyAsync(ha => ha.HotelId == hotelId && ha.UserId == userId);

            if (!isHotelAdminUser)
            {
                return Result.Failure(new Error(ErrorCodes.Forbid, "User is not authorized to perform this action."));
            }

            var booking = await context.Bookings
                .Include(b => b.Hotel)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId);

            if (booking == null)
            {
                return Result.Failure(new Error(ErrorCodes.NotFound, "Booking not found."));
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                return Result.Failure(new Error(ErrorCodes.Validation, "Booking is already cancelled."));
            }

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAtUtc = DateTime.UtcNow;
            await context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId)
        {
            var userId = usersService.GetUserId;

            var isHotelAdminUser = await context.HotelAdmins.AnyAsync(ha => ha.HotelId == hotelId && ha.UserId == userId);

            if (!isHotelAdminUser)
            {
                return Result.Failure(new Error(ErrorCodes.Forbid, "User is not authorized to perform this action."));
            }

            var booking = await context.Bookings
                .Include(b => b.Hotel)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId);

            if (booking == null)
            {
                return Result.Failure(new Error(ErrorCodes.NotFound, "Booking not found."));
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                return Result.Failure(new Error(ErrorCodes.Validation, "Cannot confirm a cancelled booking."));
            }

            if (booking.Status == BookingStatus.Confirmed)
            {
                return Result.Failure(new Error(ErrorCodes.Validation, "Booking is already confirmed."));
            }

            booking.Status = BookingStatus.Confirmed;
            booking.UpdatedAtUtc = DateTime.UtcNow;
            await context.SaveChangesAsync();

            return Result.Success();
        }
    }
}