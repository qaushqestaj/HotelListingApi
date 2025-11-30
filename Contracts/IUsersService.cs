using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListing.Api.Results;
using HotelListingApi.DTOs.Auth;

namespace HotelListingApi.Contracts
{
    public interface IUsersService
    {
        string GetUserId { get; }
        Task<Result<string>> LoginAsync(LoginUserDto loginUserDto);
        Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto);
    }
}