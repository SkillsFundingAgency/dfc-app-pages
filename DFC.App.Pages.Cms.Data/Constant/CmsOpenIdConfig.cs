using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.Pages.Cms.Data.Constant
{
    /// <summary>
    /// The CMS OpenId Config.
    /// </summary>
    public class CmsOpenIdConfig
    {
        /// <summary>
        /// The client identifier token request parameter.
        /// </summary>
        public const string ClientIdTokenRequestParam = "client_id";

        /// <summary>
        /// The client secret token request parameter.
        /// </summary>
        public const string ClientSecretTokenRequestParam = "client_secret";

        /// <summary>
        /// The grant type token request parameter.
        /// </summary>
        public const string GrantTypeTokenRequestParam = "grant_type";

        /// <summary>
        /// The grant type token request parameter value.
        /// </summary>
        public const string GrantTypeTokenRequestParamValue = "client_credentials";

        /// <summary>
        /// The authentication header bearer.
        /// </summary>
        public const string AuthHeaderBearer = "Bearer";

        /// <summary>
        /// The session API token.
        /// </summary>
        public const string SessionApiToken = "ApiToken";
    }
}
