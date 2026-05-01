using Exchange.Models;
using Exchange.Services;
using Exchange.Services.ExchangeServices.FileSource;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace ExchangeTests.Services
{
    [TestFixture]
    public class ExchangeServiceTests
    {
        private AppSettings _appSettings;
        private GlobalSettings _globalSettingsService;
        private RateService _rateService;
        private ExchangeServiceFile _exchangeService;
        private MainService _mainService;

        [SetUp]
        public async Task SetUp()
        {
            _appSettings = new() { BaseCurrency = "DKK"};
            _globalSettingsService = new();
            _rateService = new(_appSettings, _globalSettingsService);
            _exchangeService = new(_rateService);
            _mainService = new(null, null, null, null, _globalSettingsService);
        }

        [Test]
        public async Task ParseContract_CorrectArgumentsPassed_ContractCreated()
        {
            var contract = _mainService.ParseContract(new string[] { "EUR/DKK", "100" });
            Assert.IsNotNull(contract);
            Assert.AreEqual(contract.CurrencyFrom, "EUR");
            Assert.AreEqual(contract.CurrencyTo, "DKK");
            Assert.AreEqual(contract.Amount, 100);
        }

        [Test]
        public async Task ParseContract_IncorrectArgumentsPassedWithMissingAmount_ContractIsNotCreated()
        {
            var contract = _mainService.ParseContract(new string[] { "EUR/DKK" });
            Assert.IsNull(contract);
        }

        [Test]
        public async Task CalculateExchangeAmount_ContractPassed_CorrectAmountCalculated()
        {
            Dictionary<string, decimal> rates = new();
            rates.Add("EUR", 10);
            rates.Add("LTL", 5);
            _rateService.FillCustomRates(rates);

            ExchangeContract exchangeContract = new() { CurrencyFrom = "EUR", CurrencyTo = "LTL", Amount = 1 };
            var amount = await _exchangeService.CalculateExchangeAmount(exchangeContract);

            Assert.AreEqual(amount, 2);
        }

        [Test]
        public async Task CalculateExchangeAmount_ContractWithDefaultCurrencyPassed_CorrectAmountCalculated()
        {
            Dictionary<string, decimal> rates = new();
            rates.Add("EUR", 200);
            _rateService.FillCustomRates(rates);

            ExchangeContract exchangeContract = new() { CurrencyFrom = "EUR", CurrencyTo = "DKK", Amount = 1 };
            var amount = await _exchangeService.CalculateExchangeAmount(exchangeContract);

            Assert.AreEqual(amount, 2);
        }
    }
}
