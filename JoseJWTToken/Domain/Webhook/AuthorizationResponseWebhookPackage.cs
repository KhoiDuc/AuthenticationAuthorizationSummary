using JoseJWTToken.Domain.Service;

namespace JoseJWTToken.Domain.Webhook
{
    /// <summary>
    /// A Webhook package containing an authorization response
    /// </summary>
    public class AuthorizationResponseWebhookPackage : IWebhookPackage
    {
        /// <summary>
        /// The authorization response itself
        /// </summary>
        public AuthorizationResponse AuthorizationResponse { get; }

        public AuthorizationResponseWebhookPackage(AuthorizationResponse authorizationResponse)
        {
            AuthorizationResponse = authorizationResponse;
        }
    }
}