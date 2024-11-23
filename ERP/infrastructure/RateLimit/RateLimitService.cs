using ERP.infrastructure.cache;

namespace ERP.infrastructure.RateLimit
{
    public class RateLimitService : IRateLimitService
    {
        private readonly ICacheService _cacheService;

        public RateLimitService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<bool> IsRequestAllowedAsync(string userId, int maxRequests, TimeSpan timeWindow)
        {
            var bucketKey = $"user:rate_limit:{userId}";
            var timestampKey = $"user:rate_limit:{userId}:timestamp";

            // Get the current timestamp from Redis
            var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Get the current token count and the last timestamp
            var currentTokens = await _cacheService.GetAsync(bucketKey);
            var lastTimestamp = await _cacheService.GetAsync(timestampKey);

            if (string.IsNullOrEmpty(currentTokens) || string.IsNullOrEmpty(lastTimestamp))
            {
                // If there is no record, initialize with max tokens and current timestamp
                await _cacheService.SetAsync(bucketKey, maxRequests.ToString(), timeWindow);
                await _cacheService.SetAsync(timestampKey, currentTimestamp.ToString(), timeWindow);
                return true;
            }

            // Calculate how many tokens to refresh based on elapsed time
            var elapsedTime = currentTimestamp - Convert.ToInt64(lastTimestamp);

            var newTokens = Math.Min(maxRequests, Convert.ToInt32(currentTokens) + (int)(elapsedTime * (maxRequests / timeWindow.TotalSeconds)));

            if (newTokens >= 1)
            {
                // Allow request and decrement token count
                await _cacheService.SetAsync(bucketKey, (newTokens - 1).ToString(), timeWindow);
                await _cacheService.SetAsync(timestampKey, currentTimestamp.ToString(), timeWindow);
                return true;
            }

            // If there are no tokens left, deny request
            return false;
        }
    }
}
