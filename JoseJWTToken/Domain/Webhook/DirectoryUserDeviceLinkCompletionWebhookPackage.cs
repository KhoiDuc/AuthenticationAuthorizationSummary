using JoseJWTToken.Domain.Directory;

namespace JoseJWTToken.Domain.Webhook
{
    /// <summary>
    /// A Webhook package containing a device link completion response
    /// </summary>
    public class DirectoryUserDeviceLinkCompletionWebhookPackage : IWebhookPackage
    {
        /// <summary>
        /// The device link data
        /// </summary>
        public DeviceLinkCompletionResponse DeviceLinkData { get; }

        public DirectoryUserDeviceLinkCompletionWebhookPackage(DeviceLinkCompletionResponse directoryUserDeviceLinkData)
        {
            DeviceLinkData = directoryUserDeviceLinkData;
        }
    }
}