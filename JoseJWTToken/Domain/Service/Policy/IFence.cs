using System;
namespace JoseJWTToken.Domain.Service.Policy
{
    /// <summary>
    /// An interface that all Fence objects implement
    /// </summary>
    public interface IFence
    {
        String Name { get; }

        Transport.Domain.IFence ToTransport();
    }
}
