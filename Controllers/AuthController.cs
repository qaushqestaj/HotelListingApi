using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using HotelListing.Api.Results;
using HotelListingApi.Contracts;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelListingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController(IUsersService usersService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
        {
            var result = await usersService.RegisterAsync(registerUserDto);
            return ToActionResult(result);
        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginUserDto loginUserDto)
        {
            var result = await usersService.LoginAsync(loginUserDto);
            return ToActionResult(result);
        }

    }
}