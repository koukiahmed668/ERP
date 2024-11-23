namespace ERP.infrastructure.RateLimit
{
    public interface IRateLimitService
    {
        Task<bool> IsRequestAllowedAsync(string userId, int maxRequests, TimeSpan timeWindow);
    }

}
