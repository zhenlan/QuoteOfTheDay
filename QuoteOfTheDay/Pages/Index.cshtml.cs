using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace QuoteOfTheDay.Pages
{
    // The following attribute is not needed for this application to fully function. It's added to enable a client simulation app.
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class IndexModel(IOptionsSnapshot<Settings> settings, IVariantFeatureManagerSnapshot featureManager, TelemetryClient telemetryClient) : PageModel
    {
        private readonly Settings _settings = settings.Value;
        private readonly IVariantFeatureManagerSnapshot _featureManager = featureManager;
        private readonly TelemetryClient _telemetryClient = telemetryClient;

        public QuoteItem? Quote { get; set; }

        public bool ShowGreeting { get; set; }

        public async void OnGet()
        {
            Quote = _settings.Quotes[new Random().Next(_settings.Quotes.Length)];

            Variant variant = await _featureManager.GetVariantAsync("Greeting", HttpContext.RequestAborted);
            ShowGreeting = variant.Configuration.Get<bool>();

            // Simpler way to check the binary variant feature flag by using the "status_override": "Disabled" in the Off variant
            //ShowGreeting = await _featureManager.IsEnabledAsync("Greeting", default);
        }

        public IActionResult OnPostHeartQuoteAsync()
        {
            string? userId = User.Identity?.Name;

            if (!string.IsNullOrEmpty(userId))
            {
                // Send telemetry to Application Insights
                _telemetryClient.TrackEvent("HeartQuote");
                _telemetryClient.TrackMetric("QuoteHearts", 1);

                return new JsonResult(new { success = true });
            }
            else
            {
                return new JsonResult(new { success = false, error = "User not authenticated" });

            }
        }

    }
}
