using System;
using System.Collections.Generic;

namespace JoseJWTToken.Domain.Service.Policy
{
    /// <summary>
    /// An interface that all Policy objects implement
    /// </summary>
    public interface IPolicy
    {
        Boolean? DenyRootedJailbroken { get; }
        Boolean? DenyEmulatorSimulator { get; }
        List<IFence> Fences { get; }

        Transport.Domain.IPolicy ToTransport();
    }
}
