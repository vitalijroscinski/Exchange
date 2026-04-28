using Exchange.Services;
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
            ExchangeService service = new(_exchangeRatesFileName);
            var exchangeContract = service.ParseContract(args);
            if (exchangeContract == null)
            {
                Console.WriteLine("Usage: Exchange <currency pair> <amount to exchange>");
                Console.WriteLine("Example: Exchange EUR/DKK 1");
                return;
            }

            try
            {
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
