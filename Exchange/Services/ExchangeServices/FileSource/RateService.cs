using Exchange.Models;
using System;
namespace Exchange.Services.ExchangeServices.FileSource
{
    public class RateService
    {
        private readonly AppSettings _appSettings;
        private readonly GlobalSettings _globalSettingsService;

        private Dictionary<string, decimal> _rates;

        public RateService(AppSettings exchangeSettings, GlobalSettings globalSettingsService)
        {
            _appSettings = exchangeSettings;
            _globalSettingsService = globalSettingsService;
        }

        private void ValidateRate(string currency, decimal rate)
        {
            if (string.IsNullOrEmpty(currency))
                throw new Exception("Currency name is empty");

            if (currency.Trim() != currency)
                throw new Exception("Currency name contains leading or trailing spaces");

            if (rate <= 0)
                throw new Exception("Rate must be positive");
        }

        public async Task<Dictionary<string, decimal>> GetRatesAsync()
        {
            if (_rates == null)
                await FillRatesFromFileAsync();

            return _rates;
        }

        private async Task FillRatesFromFileAsync()
        {
            _rates = new(StringComparer.OrdinalIgnoreCase);

            var lines = (await File.ReadAllLinesAsync(_appSettings.RateFileName)).ToList();
            lines.Add($"{_appSettings.BaseCurrency};{_appSettings.BaseCurrency};100");

            for (int i = 1; i < lines.Count; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;

                var items = lines[i].Split(";", StringSplitOptions.TrimEntries);
                var currency = items[1];
                var rate = Decimal.Parse(items[2].Replace(",", "."), _globalSettingsService.NumberFormatInfo) / 100m;

                ValidateRate(currency, rate);

                if (_rates.ContainsKey(currency))
                {
                    if (_rates[currency] != rate)
                        throw new Exception($"Currency '{currency}' has multiple rates in the file '{_appSettings.RateFileName}'");
                    else
                        continue;
                }

                _rates.Add(currency, rate);
            }
        }

        public void FillCustomRates(Dictionary<string, decimal> rates)
        {
            _rates = new(StringComparer.OrdinalIgnoreCase);
            foreach (var rate in rates)
            {
                ValidateRate(rate.Key, rate.Value);
                _rates.Add(rate.Key, rate.Value);
            }

            if (!_rates.ContainsKey(_appSettings.BaseCurrency))
                _rates.Add(_appSettings.BaseCurrency, 100);
        }
    }
}
