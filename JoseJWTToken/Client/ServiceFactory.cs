﻿using JoseJWTToken.Transport;
using System;

namespace JoseJWTToken.Client
{
    /// <summary>
    /// Creates API clients for a given Service. This class allows the programmer to interact with a service via IServiceClient, which this class creates.
    /// </summary>
    public class ServiceFactory
    {
        private ITransport _transport;
        private Guid _serviceId;

        public ServiceFactory(ITransport transport, Guid serviceId)
        {
            _transport = transport;
            _serviceId = serviceId;
        }

        /// <summary>
        /// Create a service client for the service associated with this factory
        /// </summary>
        /// <returns>The service client</returns>
        public IServiceClient MakeServiceClient()
        {
            return new BasicServiceClient(_serviceId, _transport);
        }
    }
}