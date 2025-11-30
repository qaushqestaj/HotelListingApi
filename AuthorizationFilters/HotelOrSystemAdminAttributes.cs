using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelListingApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.AuthorizationFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HotelOrSystemAdminAttribute : TypeFilterAttribute
    {
        public HotelOrSystemAdminAttribute() : base(typeof(HotelOrSystemAdminFilter))
        {
        }

    }

    public class HotelOrSystemAdminFilter(HotelListingDbContext dbContext) : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpUser = context.HttpContext.User;
            if (!httpUser.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (httpUser.IsInRole("Administrator"))
            {
                return;
            }

            var userId = httpUser.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? httpUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Try to get hotelId from route data
            if (!context.RouteData.Values.TryGetValue("hotelId", out var hotelIdObj) || !int.TryParse(hotelIdObj?.ToString(), out var hotelId))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Check if the user is a Hotel Admin for the specified hotel
            var isHotelAdmin = await dbContext.HotelAdmins.AnyAsync(ha => ha.HotelId == hotelId && ha.UserId == userId);
            if (!isHotelAdmin)
            {
                context.Result = new ForbidResult();
                return;
            }
        }

    }
}