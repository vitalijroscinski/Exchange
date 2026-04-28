using Exchange.Models;
using Exchange.Services;
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

            var settings = config.GetSection("Rates").Get<ExchangeSettings>();

            //Dependency injection
            var serviceProvider = new ServiceCollection()
                .AddScoped<ExchangeService>()
                .AddScoped<RateService>()
                .AddSingleton<GlobalSettingsService>()
                .AddSingleton(settings)
                .BuildServiceProvider();

            try
            {

                var service = serviceProvider.GetService<ExchangeService>();
                var rateService = serviceProvider.GetService<RateService>();
                await rateService.FillRatesFromFileAsync(settings.RateFileName);


                //ExchangeService service = new(settings);
                var exchangeContract = service.ParseContract(args);
                if (exchangeContract == null)
                {
                    Console.WriteLine("Usage: Exchange <currency pair> <amount to exchange>");
                    Console.WriteLine("Example: Exchange EUR/DKK 1");
                    return;
                }

                var amount = service.CalculateExchangeAmount(exchangeContract);
                Console.WriteLine(amount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not calculate. Error message:{ex.Message}");
            }
        }
    }
}
