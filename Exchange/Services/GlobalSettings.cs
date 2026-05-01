using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Exchange.Services
{
    public class GlobalSettings
    {
        public NumberFormatInfo NumberFormatInfo = new();

        public GlobalSettings()
        {
            NumberFormatInfo.NumberDecimalSeparator = ".";
        }
    }
}
