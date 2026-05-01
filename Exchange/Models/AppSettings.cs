using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Models
{
    public class AppSettings
    {
        public string RateFileName { get; set; }
        public string RateWebApiUrl { get; set; }
        public string BaseCurrency { get; set; }
        public int RoundDigits { get; set; }
    }
}
