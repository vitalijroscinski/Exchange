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
        private Dictionary<string, decimal> _rates = new();
        private string BaseCurrency = "DKK";

        public ExchangeService(string rateFile)
        {
            _numberFormatInfo.NumberDecimalSeparator = ".";
            _rates = GetRates(rateFile);
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

        private Dictionary<string, decimal> GetRates(string fileName)
        {
            Dictionary<string, decimal> rates = new();

            var lines = File.ReadAllLines(fileName).ToList();
            lines.Add($"{BaseCurrency};{BaseCurrency};100");

            for (int i = 1; i < lines.Count; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;

                var items = lines[i].Split(";");
                var currency = items[1];
                var rate = Decimal.Parse(items[2].Replace(",", "."), _numberFormatInfo) / 100m;

                if (rates.ContainsKey(currency))
                {
                    if (rates[currency] != rate)
                        throw new Exception($"Currency '{currency}' has multiple rates in the file '{fileName}'");
                    else
                        continue;
                }

                rates.Add(currency, rate);
            }

            return rates;
        }

        public decimal CalculateExchangeAmount(ExchangeContract contract)
        {
            if (!_rates.ContainsKey(contract.CurrencyFrom))
                throw new ArgumentException($"Currency {contract.CurrencyFrom} rate is unknown");

            if (!_rates.ContainsKey(contract.CurrencyTo))
                throw new ArgumentException($"Currency {contract.CurrencyTo} rate is unknown");

            if (contract.CurrencyFrom == contract.CurrencyTo)
                return 1m;

            return _rates[contract.CurrencyFrom] / _rates[contract.CurrencyTo] * contract.Amount;
        }

    }
}
