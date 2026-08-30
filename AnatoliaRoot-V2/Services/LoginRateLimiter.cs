using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace AnatoliaRoot_V2.Services
{
    public interface ILoginRateLimiter
    {
        bool TryAcquire(string clientKey, DateTime utcNow, out TimeSpan retryAfter);
    }

    public class LoginRateLimiter : ILoginRateLimiter
    {
        private const int AttemptLimit = 10;
        private static readonly TimeSpan AttemptWindow = TimeSpan.FromMinutes(10);
        private readonly IMemoryCache _cache;

        public LoginRateLimiter(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool TryAcquire(string clientKey, DateTime utcNow, out TimeSpan retryAfter)
        {
            var attempts = _cache.GetOrCreate($"admin-login:{clientKey}", entry =>
            {
                entry.SetSize(1);
                entry.SetSlidingExpiration(AttemptWindow);
                return new Queue<DateTime>();
            });

            lock (attempts)
            {
                while (attempts.Count > 0 && utcNow - attempts.Peek() >= AttemptWindow)
                    attempts.Dequeue();

                if (attempts.Count >= AttemptLimit)
                {
                    retryAfter = AttemptWindow - (utcNow - attempts.Peek());
                    return false;
                }

                attempts.Enqueue(utcNow);
                retryAfter = TimeSpan.Zero;
                return true;
            }
        }
    }
}
