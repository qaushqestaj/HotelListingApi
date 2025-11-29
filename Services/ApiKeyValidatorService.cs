using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListingApi.Contracts;
using HotelListingApi.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Services
{
    public class ApiKeyValidatorService(HotelListingDbContext context) : IApiKeyValidatorService
    {
        public async Task<bool> IsValidApiKeyAsync(string apiKey, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) return false;

            var apiKeyEntity = await context.ApiKeys
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.Key == apiKey, ct);

            if (apiKeyEntity == null) return false;

            return apiKeyEntity.IsActive;
        }
    }
}