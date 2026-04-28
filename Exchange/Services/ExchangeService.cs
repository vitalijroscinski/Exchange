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
        private NumberFormatInfo _numberFormatInfo = new();
        private Dictionary<string, decimal> _rates = new(StringComparer.OrdinalIgnoreCase);
        private readonly ExchangeSettings _settings;

        public ExchangeService(ExchangeSettings settings)
        {
            _numberFormatInfo.NumberDecimalSeparator = ".";
            _settings = settings;
            GetRates();
        }

        public ExchangeContract? ParseContract(string[] args)
        {

            if (args == null || args.Length != 2)
                return null;

            if (!Regex.IsMatch(args[0], @"\w+/\w+"))
                return null;

            var currencies = args[0].Split("/");

            decimal amount;
            if (!Decimal.TryParse(args[1].Replace(",", "."), _numberFormatInfo, out amount))
                return null;


            ExchangeContract contract = new()
            {
                CurrencyFrom = currencies[0],
                CurrencyTo = currencies[1],
                Amount = amount
            };

            return contract;
        }

        private void GetRates()
        {
            var lines = File.ReadAllLines(_settings.RateFileName).ToList();
            lines.Add($"{_settings.BaseCurrency};{_settings.BaseCurrency};100");

            for (int i = 1; i < lines.Count; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;

                var items = lines[i].Split(";");
                var currency = items[1];
                var rate = Decimal.Parse(items[2].Replace(",", "."), _numberFormatInfo) / 100m;

                if (_rates.ContainsKey(currency))
                {
                    if (_rates[currency] != rate)
                        throw new Exception($"Currency '{currency}' has multiple rates in the file '{_settings.RateFileName}'");
                    else
                        continue;
                }

                _rates.Add(currency, rate);
            }
        }

        public decimal CalculateExchangeAmount(ExchangeContract contract)
        {
            if (!_rates.ContainsKey(contract.CurrencyFrom))
                throw new ArgumentException($"Currency {contract.CurrencyFrom} rate is unknown");

            if (!_rates.ContainsKey(contract.CurrencyTo))
                throw new ArgumentException($"Currency {contract.CurrencyTo} rate is unknown");

            if (contract.CurrencyFrom == contract.CurrencyTo)
                return decimal.Round(contract.Amount, _settings.RoundResult, MidpointRounding.ToZero);

            var amount = _rates[contract.CurrencyFrom] / _rates[contract.CurrencyTo] * contract.Amount;
            return decimal.Round(amount, _settings.RoundResult, MidpointRounding.ToZero);
        }
    }
}
