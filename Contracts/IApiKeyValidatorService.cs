using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingApi.Contracts
{
    public interface IApiKeyValidatorService
    {
        Task<bool> IsValidApiKeyAsync(string apiKey, CancellationToken ct = default);
    }
}