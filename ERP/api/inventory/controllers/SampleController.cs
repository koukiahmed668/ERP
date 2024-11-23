using ERP.infrastructure.cache;
using ERP.infrastructure.RateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.api.inventory.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly IRateLimitService _rateLimitService;

        public SampleController(ICacheService cacheService, IRateLimitService rateLimitService)
        {
            _cacheService = cacheService;
            _rateLimitService = rateLimitService;
        }

        [HttpGet("cache-test")]
        public async Task<IActionResult> CacheTest()
        {
            var key = "test-key";
            var value = "Hello, Redis!";
            var expiration = TimeSpan.FromMinutes(10);

            // Store in cache
            await _cacheService.SetAsync(key, value, expiration);

            // Retrieve from cache
            var cachedValue = await _cacheService.GetAsync(key);

            return Ok(new { key, cachedValue });
        }

        [HttpGet("bucket-test")]
        public async Task<IActionResult> CacheTest([FromHeader] string userId)
        {
            var maxRequests = 100;  // Maximum requests per minute
            var timeWindow = TimeSpan.FromMinutes(1);  // Time window for rate limit (e.g., 1 minute)

            var isAllowed = await _rateLimitService.IsRequestAllowedAsync(userId, maxRequests, timeWindow);

            if (!isAllowed)
            {
                return StatusCode(429, "Too many requests. Please try again later.");
            }

            // Handle normal logic here
            var key = "test-key";
            var value = "Hello, Redis!";
            var expiration = TimeSpan.FromMinutes(10);

            await _cacheService.SetAsync(key, value, expiration);

            return Ok(new { message = "Request allowed" });
        }

    }
}
