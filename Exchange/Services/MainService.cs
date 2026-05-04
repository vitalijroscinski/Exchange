using Exchange.Interfaces;
using Exchange.Models;
using Exchange.Services.ExchangeServices.FileSource;
using Exchange.Services.ExchangeServices.NetworkSource;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Exchange.Services
{
    public class MainService
    {
        private readonly IEnumerable<IExchangeService> _exchangeServices;
        private readonly RateService _rateService;
        private readonly AppSettings _appSettings;
        private readonly GlobalSettings _globalSettings;

        public MainService(IEnumerable<IExchangeService> exchangeServices,
            RateService rateService, AppSettings exchangeSettings, GlobalSettings globalSettings)
        {
            _exchangeServices = exchangeServices;
            _rateService = rateService;
            _appSettings = exchangeSettings;
            _globalSettings = globalSettings;
        }

        public async Task Run(string[] args)
        {
            try
            {
                var exchangeContract = ParseContract(args);
                if (exchangeContract == null)
                {
                    Console.WriteLine("Usage: Exchange <currency pair> <amount to exchange>");
                    Console.WriteLine("Example: Exchange EUR/DKK 1");
                    return;
                }

                Console.WriteLine("Select currences source:");

                var servicesCount = _exchangeServices.Count();

                for (int i = 0; i < servicesCount; i++)
                    Console.WriteLine($"  {i + 1} {_exchangeServices.ElementAt(i).ExchangeRatesSource}");

                Console.WriteLine($"  {servicesCount + 1}. All");

                var keyInt = ReadKey(servicesCount);

                if (keyInt <= servicesCount)
                {
                    await CalculateExchangeAmount(_exchangeServices.ElementAt(keyInt - 1), exchangeContract);
                }
                else if (keyInt == servicesCount + 1)
                {
                    var tasks = _exchangeServices.Select(async s => await CalculateExchangeAmount(s, exchangeContract));
                    await Task.WhenAll(tasks);
                }
                else
                {
                    throw new ArgumentException("Incorrect key");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not calculate. Error message:{ex.Message}");
            }
        }

        private async Task CalculateExchangeAmount(IExchangeService service, ExchangeContract contract)
        {
            var amount = await service.CalculateExchangeAmount(contract);
            Console.WriteLine($"Source {service.ExchangeRatesSource}. Amount: {Math.Round(amount, _appSettings.RoundDigits, MidpointRounding.ToZero)}");
        }

        private int ReadKey(int servicesCount)
        {
            var key = Console.ReadKey(true);
            int keyInt = -1;

            if (!int.TryParse(key.KeyChar.ToString(), out keyInt))
                throw new Exception("Key is not number");

            return keyInt;
        }

        public ExchangeContract? ParseContract(string[] args)
        {
            if (args == null || args.Length != 2)
                return null;

            if (!Regex.IsMatch(args[0], @"\w+/\w+"))
                return null;

            var currencies = args[0].Split("/");

            decimal amount;
            if (!Decimal.TryParse(args[1].Replace(",", "."), _globalSettings.NumberFormatInfo, out amount))
                return null;


            ExchangeContract contract = new()
            {
                CurrencyFrom = currencies[0],
                CurrencyTo = currencies[1],
                Amount = amount
            };

            return contract;
        }
    }
}
