using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.RedisCacheService.Enum
{
    /// <summary>
    /// Cache Action Enum.
    /// </summary>
    public enum CacheActionEnum
    {
        /// <summary>
        /// None action.
        /// </summary>
        None = 0,

        /// <summary>
        /// Add action.
        /// </summary>
        Add = 1,

        /// <summary>
        /// Read action.
        /// </summary>
        Read = 2,
    }
}
