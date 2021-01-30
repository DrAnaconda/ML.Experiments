using Microsoft.ML.Data;
using ML.Experiments.API.Abstractions;
using System;

namespace ML.Experiments.API.Models
{
    public abstract class DateBase : IDateRecord
    {
        #region Time utilities/converters

        public int year { get { return (int)(Date / 100000000); } }
        public int month { get { return (int)(Date / 1000000) % 100; } }
        public int day { get { return (int)(Date / 10000) % 100; } }
        public int hour { get { return (int)(Date / 100) % 100; } }
        public int minute { get { return (int)(Date % 100); } }

        #endregion

        public int secondsAfterMidnight { get { return (hour * 60 * 60) + (minute * 60); } }
        public int minutesAfterMidnight { get { return (hour * 60) + minute; } }
        public long timestamp { get { return ((DateTimeOffset)new DateTime(year, month, day, hour, minute, 0)).ToUnixTimeSeconds(); } }

        public long Date { get; set; }
    }
}

// 1000 00 00 00 00
// 2020 07 10 22 30
// 2020 07 

// 2020 02 02 03 19 => Dt from Sleep Records
// 2018 08 29 05 10 => Dt from hr
// 2018 09 30 23 00 => Dt from main records