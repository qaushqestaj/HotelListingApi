using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListingApi.DTOs.Hotel;

namespace HotelListingApi.Contracts
{
    public interface IHotelsService
    {
        Task<bool> HotelExists(int id);
        Task<bool> HotelExists(string name);
        Task<IEnumerable<GetHotelDto>> GetHotels();
        Task<GetHotelDto?> GetHotelById(int id);
        Task PutHotel(int id, UpdateHotelDto model);
        Task<GetHotelDto> PostHotel(CreateHotelDto model);
        Task DeleteHotelById(int id);
    }
}