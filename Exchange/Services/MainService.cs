using Exchange.Interfaces;
using Exchange.Models;
using Exchange.Services.ExchangeServices.FileSource;
using Exchange.Services.ExchangeServices.NetworkSource;
using System.Text.RegularExpressions;

namespace Exchange.Services
{
    public class MainService
    {
        private readonly ExchangeServiceFile _exchangeServiceFile;
        private readonly ExchangeServiceNetwork _exchangeServiceNetwork;
        private readonly RateService _rateService;
        private readonly AppSettings _appSettings;
        private readonly GlobalSettings _globalSettings;

        public MainService(ExchangeServiceFile exchangeServiceFile,
            ExchangeServiceNetwork exchangeServiceNetwork,

            RateService rateService, AppSettings exchangeSettings, GlobalSettings globalSettings)
        {
            _exchangeServiceFile = exchangeServiceFile;
            _exchangeServiceNetwork = exchangeServiceNetwork;
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

                Console.WriteLine("Select currences source");
                Console.WriteLine($"1. File {_appSettings.RateFileName}");
                Console.WriteLine($"2. Url {_appSettings.RateWebApiUrl}");
                var key = Console.ReadKey();

                IExchangeService exchangeService = null;
                switch (key.KeyChar)
                {
                    case '1':
                        exchangeService = _exchangeServiceFile;
                        break;

                    case '2':
                        exchangeService = _exchangeServiceNetwork;
                        break;

                    default:
                        throw new ArgumentException("Selected value is incorrect");

                }

                var amount = await exchangeService.CalculateExchangeAmount(exchangeContract);
                Console.WriteLine();
                Console.WriteLine($"Amount: {Math.Round(amount, _appSettings.RoundDigits, MidpointRounding.ToZero)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not calculate. Error message:{ex.Message}");
            }
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
