using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Models
{
    public class ExchangeSettings
    {
        public string RateFileName { get; set; }
        public string BaseCurrency { get; set; }
        public int RoundResult { get; set; }
    }
}
