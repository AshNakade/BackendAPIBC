using Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace BCFunctions
{
    public  class Progrma
    {
        public static  async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureServices(services =>
                {
                    DependencyInjection.AddApplicationDependencies(services);
                })
                .ConfigureLogging(logging =>
                {
					logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
				})
                .Build();
            await host.RunAsync();
        }
    }
}