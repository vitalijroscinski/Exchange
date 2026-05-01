using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Exchange.Services.ExchangeServices.NetworkSource.Models
{
    public class RatesResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<string, string> Rates { get; set; }
    }
}
