using Exchange.Interfaces;
using Exchange.Models;
using Exchange.Services.ExchangeServices.NetworkSource.Models;
using System.Text.Json;

namespace Exchange.Services.ExchangeServices.NetworkSource
{
    public class ExchangeServiceNetwork : IExchangeService
    {
        private readonly AppSettings _appSettings;
        private readonly GlobalSettings _globalSettings;

        public ExchangeServiceNetwork(AppSettings exchangeSettings, GlobalSettings globalSettings)
        {
            _appSettings = exchangeSettings;
            _globalSettings = globalSettings;
        }

        public async Task<decimal> CalculateExchangeAmount(ExchangeContract contract)
        {
            Func<string, decimal> ToDecimal = value => Decimal.Parse(value, _globalSettings.NumberFormatInfo);

            UriBuilder uriBuilder = new(_appSettings.RateWebApiUrl);
            var query = System.Web.HttpUtility.ParseQueryString(_appSettings.RateWebApiUrl);
            query["base"] = _appSettings.BaseCurrency;
            query["codes"] = string.Join(",", contract.CurrencyFrom, contract.CurrencyTo);

            uriBuilder.Query = query.ToString();

            HttpClient httpClient = new();
            var response = await httpClient.GetAsync(uriBuilder.Uri);
            var result = await response.Content.ReadAsStringAsync();

            var rates = JsonSerializer.Deserialize<RatesResponse>(result);

            if (!rates.Success)
                throw new Exception("Converstion was not possible");

            rates.Rates = new Dictionary<string, string>(rates.Rates, StringComparer.OrdinalIgnoreCase);


            var exchangeAmount = ToDecimal(rates.Rates[contract.CurrencyTo]) / ToDecimal(rates.Rates[contract.CurrencyFrom]) * contract.Amount;
            return exchangeAmount;
        }

    }
}
