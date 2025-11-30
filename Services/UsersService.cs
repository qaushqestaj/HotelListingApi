using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HotelListing.Api.Results;
using HotelListingApi.Common.Constans;
using HotelListingApi.Contracts;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HotelListingApi.Services
{
    public class UsersService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, HotelListingDbContext hotelListingDbContext) : IUsersService
    {

        public async Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto)
        {
            var user = new ApplicationUser
            {
                UserName = registerUserDto.Email,
                Email = registerUserDto.Email,
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName
            };

            var result = await userManager.CreateAsync(user, registerUserDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
                return Result<RegisteredUserDto>.BadRequest(errors);
            }

            await userManager.AddToRoleAsync(user, registerUserDto.Role);

            if (registerUserDto.Role == "Hotel Admin")
            {
                var hotelAdmin = hotelListingDbContext.HotelAdmins.Add(new HotelAdmin
                {
                    UserId = user.Id,
                    HotelId = registerUserDto.AssociatedHotelId.GetValueOrDefault()
                });

                await hotelListingDbContext.SaveChangesAsync();
            }

            var registeredUserDto = new RegisteredUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = registerUserDto.Role
            };

            return Result<RegisteredUserDto>.Success(registeredUserDto);
        }



        public async Task<Result<string>> LoginAsync(LoginUserDto loginUserDto)
        {
            var user = await userManager.FindByEmailAsync(loginUserDto.Email);
            if (user == null)
            {
                return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid email or password."));
            }

            var isPasswordValid = await userManager.CheckPasswordAsync(user, loginUserDto.Password);
            if (!isPasswordValid)
            {
                return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid email or password."));

            }

            // Issue a token
            var token = await GenerateToken(user);

            return Result<string>.Success(token);
        }

        public string GetUserId => httpContextAccessor?.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;


        private async Task<string> GenerateToken(ApplicationUser user)
        {
            // Token generation logic goes here

            // Set basic user claims
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Name, user.FullName)
            };

            // Add user roles as claims
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            claims = claims.Union(roleClaims).ToList();

            // Set JWT Key credentials
            var securityKey = new SymmetricSecurityKey(
           Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the token
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}