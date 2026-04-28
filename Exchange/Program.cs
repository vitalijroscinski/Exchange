using Exchange.Models;
using Exchange.Services;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Exchange
{
    internal class Program
    {
        private const string _exchangeRatesFileName = "DkkExchangeRates.csv";
        private static NumberFormatInfo _numberFormatInfo = new();
        static async Task Main(string[] args)
        {
            try
            {
                // Build configuration
                IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory) // Ensure correct path
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var settings = config.GetSection("Rates").Get<ExchangeSettings>();
                if (settings == null)
                    throw new Exception("Problem with reading configurtion file");

                ExchangeService service = new(settings);
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
