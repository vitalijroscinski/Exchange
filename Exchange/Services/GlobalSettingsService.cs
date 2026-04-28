using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Exchange.Services
{
    public class GlobalSettingsService
    {
        public NumberFormatInfo NumberFormatInfo = new();

        public GlobalSettingsService()
        {
            NumberFormatInfo.NumberDecimalSeparator = ".";
        }
    }
}
