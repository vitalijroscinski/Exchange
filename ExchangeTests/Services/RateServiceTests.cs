using Exchange.Services.ExchangeServices.FileSource;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeTests.Services
{
    [TestFixture]
    public class RateServiceTests
    {
        private RateService _rateService;

        [SetUp]
        public void SetUp()
        {
            var exchangeSettings = new Exchange.Models.AppSettings() {BaseCurrency="DKK"};
            _rateService = new(exchangeSettings, null);
        }

        [Test]
        public async Task FillCustomRates_AddCustomRate_RateExists()
        {

            Dictionary<string, decimal> rates = new();
            rates.Add("EUR", 100);
            _rateService.FillCustomRates(rates);
            var serviceRates = await _rateService.GetRatesAsync();

            Assert.IsTrue(serviceRates.ContainsKey("EUR"));
            Assert.AreEqual(serviceRates["EUR"], 100);
        }

        [Test]
        public async Task FillCustomRates_AddCustomRate_RateIsCaseInsensetive()
        {
            Dictionary<string, decimal> rates = new();
            rates.Add("EUR", 100);
            _rateService.FillCustomRates(rates);
            var serviceRates = await _rateService.GetRatesAsync();

            Assert.IsTrue(serviceRates.ContainsKey("eur"));
        }

        [Test]
        public void FillCustomRates_AddEmptyCurrencyName_ThrowException()
        {
            Dictionary<string, decimal> rates = new();
            rates.Add("", 100);
            Assert.Throws<Exception>(() => _rateService.FillCustomRates(rates));
        }

        [Test]
        public void FillCustomRates_AddZeroRate_ThrowException()
        {
            Dictionary<string, decimal> rates = new();
            rates.Add("EUR", 0);
            Assert.Throws<Exception>(() => _rateService.FillCustomRates(rates));
        }

        [Test]
        public void FillCustomRates_AddNegativeRate_ThrowException()
        {
            Dictionary<string, decimal> rates = new();
            rates.Add("EUR", -1);
            Assert.Throws<Exception>(() => _rateService.FillCustomRates(rates));
        }

        [Test]
        public void FillCustomRates_AddCurrencyNameWithTrailingSpace_ThrowException()
        {
            Dictionary<string, decimal> rates = new();
            rates.Add(" EUR", 100);
            Assert.Throws<Exception>(() => _rateService.FillCustomRates(rates));
        }

        [Test]
        public void FillCustomRates_AddCurrencyNameWithEndingSpace_ThrowException()
        {
            Dictionary<string, decimal> rates = new();
            rates.Add("EUR ", 100);
            Assert.Throws<Exception>(() => _rateService.FillCustomRates(rates));
        }
    }
}
