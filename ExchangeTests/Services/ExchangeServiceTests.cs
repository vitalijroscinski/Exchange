using Exchange.Models;
using Exchange.Services;
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
        private ExchangeSettings _exchangeSettings;
        private GlobalSettingsService _globalSettingsService;
        private RateService _rateService;
        private ExchangeService _exchangeService;

        [SetUp]
        public void SetUp()
        {
            _exchangeSettings = new() { BaseCurrency = "DKK", RoundResult = 2 };
            _globalSettingsService = new();
            _rateService = new(_exchangeSettings, _globalSettingsService);
            _exchangeService = new(_exchangeSettings, _rateService, _globalSettingsService);

        }

        [Test]
        public void ParseContract_CorrectArgumentsPassed_ContractCreated()
        {
            var contract = _exchangeService.ParseContract(new string[] { "EUR/DKK", "100" });
            Assert.IsNotNull(contract);
            Assert.AreEqual(contract.CurrencyFrom, "EUR");
            Assert.AreEqual(contract.CurrencyTo, "DKK");
            Assert.AreEqual(contract.Amount, 100);
        }

        [Test]
        public void ParseContract_IncorrectArgumentsPassed_ContractIsNotCreated()
        {
            var contract = _exchangeService.ParseContract(new string[] { "EUR/DKK"});
            Assert.IsNull(contract);
        }

        [Test]
        public void CalculateExchangeAmount_ContractPassed_CorrectAmountCalculated()
        {
            Dictionary<string, decimal> rates = new();
            rates.Add("EUR", 10);
            rates.Add("LTL", 5);
            _rateService.FillCustomRates(rates);

            ExchangeContract exchangeContract = new() {CurrencyFrom="EUR", CurrencyTo="LTL", Amount=1 };
            var amount = _exchangeService.CalculateExchangeAmount(exchangeContract);

            Assert.AreEqual(amount, 2);
        }

        [Test]
        public void CalculateExchangeAmount_ContractWithDefaultCurrencyPassed_CorrectAmountCalculated()
        {
            Dictionary<string, decimal> rates = new();
            rates.Add("EUR", 10.1m);
            _rateService.FillCustomRates(rates);

            ExchangeContract exchangeContract = new() { CurrencyFrom = "EUR", CurrencyTo = "DKK", Amount = 1 };
            var amount = _exchangeService.CalculateExchangeAmount(exchangeContract);

            Assert.AreEqual(amount, 0.1m);
        }
    }
}
