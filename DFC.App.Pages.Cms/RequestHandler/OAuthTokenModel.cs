using Newtonsoft.Json;

namespace DFC.App.Pages.Cms.RequestHandler
{
    /// <summary>
    /// The OAuth Token model.
    /// </summary>
    public class OAuthTokenModel
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        /// <value>
        /// The type of the token.
        /// </value>
        [JsonProperty("token_type")]

        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the expires in.
        /// </summary>
        /// <value>
        /// The expires in.
        /// </value>
        [JsonProperty("expires_in")]

        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the expiry datetime.
        /// </summary>
        /// <value>
        /// The expiry datetime.
        /// </value>
        public DateTime ExpiryDatetime { get; set; }
    }
}
