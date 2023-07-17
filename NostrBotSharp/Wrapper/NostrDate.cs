using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostrBotSharp.Wrapper
{
    public class NostrDate
    {
        public static string ConvertToString(DateTime? datetime)
        {
            TimeZoneInfo tzi = TimeZoneInfo.Local;
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc((DateTime)datetime, tzi);
            return localTime.ToString("yyyy/MM/dd(ddd) HH:mm:ss");
        }
    }
}
