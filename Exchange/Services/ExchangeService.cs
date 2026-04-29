using Exchange.Extensions;
using Exchange.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Exchange.Services
{
    public class ExchangeService
    {
        private readonly RateService _rateService;
        private readonly GlobalSettingsService _globalSettingsService;

        public ExchangeService(RateService rateService, GlobalSettingsService globalSettingsService)
        {
            _rateService = rateService;
            _globalSettingsService = globalSettingsService;
        }

        public ExchangeContract? ParseContract(string[] args)
        {

            if (args == null || args.Length != 2)
                return null;

            if (!Regex.IsMatch(args[0], @"\w+/\w+"))
                return null;

            var currencies = args[0].Split("/");

            decimal amount;
            if (!Decimal.TryParse(args[1].Replace(",", "."), _globalSettingsService.NumberFormatInfo, out amount))
                return null;


            ExchangeContract contract = new()
            {
                CurrencyFrom = currencies[0],
                CurrencyTo = currencies[1],
                Amount = amount
            };

            return contract;
        }

        public decimal CalculateExchangeAmount(ExchangeContract contract)
        {
            if (!_rateService.Rates.ContainsKey(contract.CurrencyFrom))
                throw new ArgumentException($"Currency {contract.CurrencyFrom} rate is unknown");

            if (!_rateService.Rates.ContainsKey(contract.CurrencyTo))
                throw new ArgumentException($"Currency {contract.CurrencyTo} rate is unknown");

            if (contract.CurrencyFrom == contract.CurrencyTo)
                return contract.Amount.DefaultRound();

            var amount = _rateService.Rates[contract.CurrencyFrom] / _rateService.Rates[contract.CurrencyTo] * contract.Amount;
            return amount.DefaultRound();
        }
    }
}
