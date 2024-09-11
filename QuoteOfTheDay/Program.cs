using Azure.Identity;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Telemetry.ApplicationInsights;
using QuoteOfTheDay;
using QuoteOfTheDay.Authentication;

var builder = WebApplication.CreateBuilder(args);


// Load configuration from Azure App Configuration
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(new Uri(Environment.GetEnvironmentVariable("AZUREAPPCONFIGURATION_ENDPOINT")), new DefaultAzureCredential())
           // Load all keys that start with `QuoteOfTheDay:` and have no label
           .Select("QuoteOfTheDay:*")
           .ConfigureRefresh(refreshOptions =>
           {
               refreshOptions.Register("QuoteOfTheDay:Quotes", refreshAll: true);
           })
           // Load all feature flags that have keys starting with "QuoteOfTheDay:" with no label.
           .UseFeatureFlags(featureFlagOptions =>
           {
               featureFlagOptions.Select("QuoteOfTheDay:*");
               featureFlagOptions.SetRefreshInterval(TimeSpan.FromSeconds(5));
           });
});

// Add Application Insights telemetry with adaptive sampling disabled.
builder.Services.AddApplicationInsightsTelemetry( new ApplicationInsightsServiceOptions() { EnableAdaptiveSampling = false})
                .AddSingleton<ITelemetryInitializer, TargetingTelemetryInitializer>();

// Add services to the container.
builder.Services.AddRazorPages();

// Add query string-based authentication
builder.Services.AddAuthentication(Schemes.QueryString)
                .AddQueryString();

builder.Services.AddHttpContextAccessor();

// Add Azure App Configuration and feature management services to the container.
builder.Services.AddAzureAppConfiguration()
                .AddFeatureManagement()
                .WithTargeting()
                .AddApplicationInsightsTelemetryPublisher();

// Bind configuration to the Settings object
builder.Services.Configure<Settings>(builder.Configuration.GetSection("QuoteOfTheDay"));

// Allows cross-origin requests (CORS) from the client-side. Used for the client simulation app.
builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                });
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use Azure App Configuration middleware for dynamic configuration refresh.
app.UseAzureAppConfiguration();

// Use middleware to cache TargetingId in HttpContext used for telemetry
app.UseMiddleware<TargetingHttpContextMiddleware>();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseCors();

app.Run();
