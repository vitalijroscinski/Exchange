using Exchange.Models;
using System;
namespace Exchange.Services
{
    public class RateService
    {
        private readonly ExchangeSettings _settings;
        private readonly GlobalSettingsService _globalSettingsService;
        private Dictionary<string, decimal> _rates = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, decimal> Rates => _rates;

        public RateService(ExchangeSettings settings, GlobalSettingsService globalSettingsService)
        {
            _settings = settings;
            _globalSettingsService = globalSettingsService;
        }

        private void ValidateRate(string currency, decimal rate)
        {
            if (string.IsNullOrEmpty(currency))
                throw new Exception("Currency name is empty");

            if(currency.Trim()!=currency)
                throw new Exception("Currency name contains leading or trailing spaces");

            if (rate <= 0)
                throw new Exception("Rate must be positive");
        }

        public async Task FillRatesFromFileAsync(string fileName)
        {
            var lines = (await File.ReadAllLinesAsync(_settings.RateFileName)).ToList();
            lines.Add($"{_settings.BaseCurrency};{_settings.BaseCurrency};100");

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
                        throw new Exception($"Currency '{currency}' has multiple rates in the file '{_settings.RateFileName}'");
                    else
                        continue;
                }

                _rates.Add(currency, rate);
            }
        }

        public void FillCustomRates(Dictionary<string, decimal> rates)
        {
            foreach (var rate in rates)
            {
                ValidateRate(rate.Key, rate.Value);
                _rates.Add(rate.Key, rate.Value);
            }

            if (!_rates.ContainsKey(_settings.BaseCurrency))
                _rates.Add(_settings.BaseCurrency, 100);
        }
    }
}
