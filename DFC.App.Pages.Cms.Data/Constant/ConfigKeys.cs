using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.Pages.Cms.Data.Constant
{
    public class ConfigKeys
    {
        // open id config keys

        /// <summary>
        /// The token end point URL.
        /// </summary>
        public const string TokenEndPointUrl = "Cms:TokenEndPointUrl";

        /// <summary>
        /// The client identifier.
        /// </summary>
        public const string ClientId = "Cms:ClientId";

        /// <summary>
        /// The client secret.
        /// </summary>
        public const string ClientSecret = "Cms:ClientSecret";

        /// <summary>
        /// The API Url for the CMS.
        /// </summary>
        public const string GraphApiUrl = "Cms:GraphApiUrl";

        public const string SqlApiUrl = "Cms:SqlApiUrl";
    }
}
