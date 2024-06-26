﻿using System;
using System.Collections.Generic;

namespace JoseJWTToken.Domain.Service.Policy
{
    /// <summary>
    /// Object that represents a method amount Authorization Policy
    /// </summary>
    public class MethodAmountPolicy : IPolicy
    {
        /// <summary>
        /// The amount of factors required for the Authorization Request to be valid
        /// </summary>
        public Double Amount { get; }

        /// <summary>
        /// Whether to allow or deny rooted or jailbroken devices
        /// </summary>
        public bool? DenyRootedJailbroken { get; }

        /// <summary>
        /// Whether to allow or deny emulator or simulator devices
        /// </summary>
        public bool? DenyEmulatorSimulator { get; }

        /// <summary>
        /// List containing any Fence objects for the Authorization Policy
        /// </summary>
        public List<IFence> Fences { get; }

        public MethodAmountPolicy(
            List<IFence> fences,
            Double amount = 0,
            bool? denyRootedJailbroken = false,
            bool? denyEmulatorSimulator = false
            )
        {
            Amount = amount;
            DenyRootedJailbroken = denyRootedJailbroken;
            DenyEmulatorSimulator = denyEmulatorSimulator;
            Fences = fences ?? new List<IFence>();
        }

        /// <summary>
        /// Returns the Transport object that can be used in the transport for
        /// sending to the LaunchKey API
        /// </summary>
        /// <returns>Returns this objects representation to Sdk.Transport.Domain.IPolicy</returns>
        public Transport.Domain.IPolicy ToTransport()
        {
            List<Transport.Domain.IFence> fences = new List<Transport.Domain.IFence>();
            foreach (IFence fence in Fences)
            {
                fences.Add(fence.ToTransport());
            }

            return new Transport.Domain.MethodAmountPolicy(
                amount: Amount,
                denyRootedJailbroken: DenyRootedJailbroken,
                denyEmulatorSimulator: DenyEmulatorSimulator,
                fences: fences
            );
        }
    }
}
