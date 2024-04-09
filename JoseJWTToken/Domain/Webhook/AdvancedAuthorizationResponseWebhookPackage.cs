using JoseJWTToken.Domain.Service;

namespace JoseJWTToken.Domain.Webhook
{
    /// <summary>
    /// A Webhook package containing an authorization response
    /// </summary>
    public class AdvancedAuthorizationResponseWebhookPackage : IWebhookPackage
    {
        /// <summary>
        /// The authorization response itself
        /// </summary>
        public AdvancedAuthorizationResponse AdvancedAuthorizationResponse { get; }

        public AdvancedAuthorizationResponseWebhookPackage(AdvancedAuthorizationResponse authorizationResponse)
        {
            AdvancedAuthorizationResponse = authorizationResponse;
        }
    }
}