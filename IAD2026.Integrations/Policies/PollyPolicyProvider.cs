using Polly;
using Polly.Retry;

namespace IAD2026.Integrations.Policies;

public interface IPollyPolicyProvider
{
    AsyncRetryPolicy<HttpResponseMessage> GetExternalApiRetryPolicy();
}

public class PollyPolicyProvider : IPollyPolicyProvider
{
    public AsyncRetryPolicy<HttpResponseMessage> GetExternalApiRetryPolicy()
    {
        return Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r =>
                (int)r.StatusCode >= 500 || r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}