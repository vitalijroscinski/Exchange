using Exchange.Models;
using Exchange.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Extensions
{
    public static class ExchangeExtensions
    {
        public static ExchangeSettings ExchangeSettings { get; set; }
        public static decimal DefaultRound(this decimal value)
        {
            return decimal.Round(value, ExchangeSettings.RoundResult, MidpointRounding.ToZero);
        }
    }
}
