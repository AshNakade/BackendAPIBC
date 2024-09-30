using Application.Services.CustoemrSearch;
using Application.Services.CPInvoices;
using Application.Services.StoreBalanceSummary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Identity.Web;
namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            services.AddApplicationInsightsTelemetryWorkerService();
            services.ConfigureFunctionsApplicationInsights();
            services.AddTransient<ICustomersSearch, CustomerSearch>();
            services.AddTransient<ICPInvoicesService, CPInvoicesService>();
           services.AddTransient<IStoreBalanceSummaryService, StoreBalanceSummaryService>();
			services.AddHttpClient();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"))
           .EnableTokenAcquisitionToCallDownstreamApi()
           .AddDownstreamApi("BCAPIDownStream", configuration.GetSection("BCAPI"))
           .AddInMemoryTokenCaches();
            return services;
        }
    }
}