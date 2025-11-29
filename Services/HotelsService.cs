using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListingApi.Contracts;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Hotel;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Services;

public class HotelsService(HotelListingDbContext context, IMapper mapper) : IHotelsService
{
    public async Task<IEnumerable<GetHotelDto>> GetHotels()
    {
        var hotels = await context.Hotels
            .Select(h => new GetHotelDto(h.Id, h.Name, h.Address, h.Rating, h.CountryId, h.Country!.Name)).ToListAsync();
        return hotels;
    }

    public async Task<GetHotelDto?> GetHotelById(int id)
    {
        var hotel = await context.Hotels
        .Where(h => h.Id == id)
        .Include(h => h.Country)
        .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
        // .Select(h => new GetHotelDto(h.Id, h.Name, h.Address, h.Rating, h.CountryId, h.Country!.Name))
        .FirstOrDefaultAsync();

        return hotel ?? throw new KeyNotFoundException("Hotel not found");
    }

    public async Task PutHotel(int id, UpdateHotelDto model)
    {
        var hotel = await context.Hotels.FindAsync(id) ?? throw new KeyNotFoundException("Hotel not found");

        var updatedHotel = mapper.Map<Hotel>(hotel);

        context.Hotels.Update(updatedHotel);

        await context.SaveChangesAsync();
    }

    public async Task<GetHotelDto> PostHotel(CreateHotelDto model)
    {
        var hotel = mapper.Map<Hotel>(model);
        // var hotel = new Hotel
        // {
        //     Name = model.Name,
        //     Address = model.Address,
        //     Rating = model.Rating,
        //     CountryId = model.CountryId
        // };

        context.Hotels.Add(hotel);
        await context.SaveChangesAsync();
        var returnHotel = mapper.Map<GetHotelDto>(hotel);
        return returnHotel;
        // return new GetHotelDto(hotel.Id, hotel.Name, hotel.Address, hotel.Rating, hotel.CountryId, string.Empty);
    }

    public async Task DeleteHotelById(int id)
    {
        var hotel = await context.Hotels.Where(h => h.Id == id).ExecuteDeleteAsync();
    }


    public async Task<bool> HotelExists(int id)
    {
        return await context.Hotels.AnyAsync(e => e.Id == id);
    }
    public async Task<bool> HotelExists(string name)
    {
        return await context.Hotels.AnyAsync(e => e.Name == name);
    }
}