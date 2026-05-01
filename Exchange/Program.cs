using Exchange.Models;
using Exchange.Services;
using Exchange.Services.ExchangeServices.FileSource;
using Exchange.Services.ExchangeServices.NetworkSource;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Exchange
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Build configuration
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var appSettings = config.GetSection("Rates").Get<AppSettings>();

            //Dependency injection
            var serviceProvider = new ServiceCollection()
                .AddSingleton<MainService>()
                .AddScoped<ExchangeServiceFile>()
                .AddScoped<ExchangeServiceNetwork>()
                .AddScoped<RateService>()
                .AddSingleton<GlobalSettings>()
                .AddSingleton(appSettings)
                .BuildServiceProvider();

            try
            {
                var mainService = serviceProvider.GetService<MainService>();
                await mainService.Run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not run application. Error message:{ex.Message}");
            }
        }
    }
}
