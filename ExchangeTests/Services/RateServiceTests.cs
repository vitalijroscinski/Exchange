using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeTests.Services
{
    [TestFixture]
    public class RateServiceTests
    {
        private Exchange.Services.RateService _rateService;

        [SetUp]
        public void SetUp()
        {
            var exchangeSettings = new Exchange.Models.ExchangeSettings() {BaseCurrency="DKK", RoundResult=2 };
            _rateService = new(exchangeSettings, null);
        }

        [Test]
        public void FillCustomRates_AddCustomRate_RateExists()
        {

            Dictionary<string, decimal> rates = new();
            rates.Add("EUR", 100);
            _rateService.FillCustomRates(rates);

            Assert.IsTrue(_rateService.Rates.ContainsKey("EUR"));
            Assert.AreEqual(_rateService.Rates["EUR"], 100);
        }

        [Test]
        public void FillCustomRates_AddCustomRate_RateIsCaseInsensetive()
        {
            Dictionary<string, decimal> rates = new();
            rates.Add("EUR", 100);
            _rateService.FillCustomRates(rates);

            Assert.IsTrue(_rateService.Rates.ContainsKey("eur"));
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


        [TearDown]
        public void TearsDown()
        {
            _rateService = null;
        }
    }
}
