using System;

namespace DFC.App.Pages.Models
{
    public class CacheReloadTimerOptions
    {
        public bool Enabled { get; set; }

        public TimeSpan DelayStart { get; set; } = TimeSpan.FromHours(1);

        public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);
    }
}
