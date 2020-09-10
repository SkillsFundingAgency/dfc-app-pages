using System;

namespace DFC.App.Pages.Models
{
    public class CacheReloadTimerOptions
    {
        public bool Enabled { get; set; } = true;

        public TimeSpan DelayStart { get; set; } = TimeSpan.FromHours(1);

        public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);
    }
}
