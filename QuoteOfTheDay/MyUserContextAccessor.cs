using Microsoft.FeatureManagement.FeatureFilters;

namespace QuoteOfTheDay
{
    public class MyUserContextAccessor(IHttpContextAccessor httpContextAccessor) : ITargetingContextAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public ValueTask<TargetingContext> GetContextAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var tc = new TargetingContext
            {
                UserId = user?.Identity?.Name,
                Groups = user?.Claims?.Select(c => c.Value)
            };

            return new ValueTask<TargetingContext>(tc);
        }
    }
}
