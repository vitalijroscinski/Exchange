using Exchange.Models;

namespace Exchange.Services
{
    public class MainService
    {
        private readonly ExchangeService _exchangeService;
        private readonly RateService _rateService;
        private readonly ExchangeSettings _exchangeSettings;

        public MainService(ExchangeService exchangeService, RateService rateService, ExchangeSettings exchangeSettings)
        {
            _exchangeService = exchangeService;
            _rateService = rateService;
            _exchangeSettings = exchangeSettings;
        }
        public async Task Run(string[] args)
        {
            try
            {

                await _rateService.FillRatesFromFileAsync(_exchangeSettings.RateFileName);

                var exchangeContract = _exchangeService.ParseContract(args);
                if (exchangeContract == null)
                {
                    Console.WriteLine("Usage: Exchange <currency pair> <amount to exchange>");
                    Console.WriteLine("Example: Exchange EUR/DKK 1");
                    return;
                }

                var amount = _exchangeService.CalculateExchangeAmount(exchangeContract);
                Console.WriteLine(amount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not calculate. Error message:{ex.Message}");
            }
        }
    }
}
