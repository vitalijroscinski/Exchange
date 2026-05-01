using Exchange.Interfaces;
using Exchange.Models;
using System.Text.RegularExpressions;

namespace Exchange.Services.ExchangeServices.FileSource
{
    public class ExchangeServiceFile : IExchangeService
    {
        private readonly RateService _rateService;

        public ExchangeServiceFile(RateService rateService)
        {
            _rateService = rateService;
        }

       

        public async Task<decimal> CalculateExchangeAmount(ExchangeContract contract)
        {
            var rates = await _rateService.GetRatesAsync();
            if (!rates.ContainsKey(contract.CurrencyFrom))
                throw new ArgumentException($"Currency {contract.CurrencyFrom} rate is unknown");

            if (!rates.ContainsKey(contract.CurrencyTo))
                throw new ArgumentException($"Currency {contract.CurrencyTo} rate is unknown");

            if (contract.CurrencyFrom == contract.CurrencyTo)
                return contract.Amount;

            var amount = rates[contract.CurrencyFrom] / rates[contract.CurrencyTo] * contract.Amount;
            return amount;
        }
    }
}
