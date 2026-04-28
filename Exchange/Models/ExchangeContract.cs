using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Models
{
    public class ExchangeContract
    {
        public string CurrencyFrom { get; set; }
        public string CurrencyTo { get; set; }

        public decimal Amount { get; set; }
    }
}
