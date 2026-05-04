using Exchange.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Interfaces
{
    public interface IExchangeService
    {
        string ExchangeRatesSource { get; }
        Task<decimal> CalculateExchangeAmount(ExchangeContract contract);
    }
}
