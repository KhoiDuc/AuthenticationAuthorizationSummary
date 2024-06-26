﻿using JoseJWTToken.Domain;
using JoseJWTToken.Domain.Organization;
using JoseJWTToken.Domain.Service.Policy;
using JoseJWTToken.Domain.ServiceManager;
using JoseJWTToken.Transport;
using JoseJWTToken.Transport.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JoseJWTToken.Client
{
    public class BasicOrganizationClient : IOrganizationClient
    {
        private EntityIdentifier _organizationId;
        private ITransport _transport;

        public BasicOrganizationClient(Guid organizationId, ITransport transport)
        {
            _organizationId = new EntityIdentifier(EntityType.Organization, organizationId);
            _transport = transport;
        }

        public Guid CreateService(string name, string description, Uri icon, Uri callbackUrl, bool active)
        {
            var request = new ServicesPostRequest(name, description, icon, callbackUrl, active);
            var response = _transport.OrganizationV3ServicesPost(request, _organizationId);
            return response.Id;
        }

        public void UpdateService(Guid serviceId, string name, string description, Uri icon, Uri callbackUrl, bool active)
        {
            var request = new ServicesPatchRequest(serviceId, name, description, icon, callbackUrl, active);
            _transport.OrganizationV3ServicesPatch(request, _organizationId);
        }

        public Service GetService(Guid serviceId)
        {
            return GetServices(new List<Guid> { serviceId }).First();
        }

        public List<Service> GetServices(List<Guid> serviceIds)
        {
            var request = new ServicesListPostRequest(serviceIds);
            var response = _transport.OrganizationV3ServicesListPost(request, _organizationId);
            var services = new List<Service>();

            foreach (var serviceItem in response.Services)
            {
                services.Add(
                    new Service(
                        serviceItem.Id,
                        serviceItem.Name,
                        serviceItem.Description,
                        serviceItem.Icon,
                        serviceItem.CallbackUrl,
                        serviceItem.Active
                    ));
            }

            return services;
        }

        public List<Service> GetAllServices()
        {
            var response = _transport.OrganizationV3ServicesGet(_organizationId);
            var services = new List<Service>();
            foreach (var serviceItem in response.Services)
            {
                services.Add(
                    new Service(
                        serviceItem.Id,
                        serviceItem.Name,
                        serviceItem.Description,
                        serviceItem.Icon,
                        serviceItem.CallbackUrl,
                        serviceItem.Active
                    )
                );
            }

            return services;
        }

        public Guid CreateDirectory(string name)
        {
            var request = new OrganizationV3DirectoriesPostRequest(name);
            var response = _transport.OrganizationV3DirectoriesPost(request, _organizationId);
            return response.Id;
        }

        public void UpdateDirectory(Guid directoryId, bool active, string androidKey, string iosP12, bool? denialContextInquiryEnabled = null, string webhookUrl = null)
        {
            var request = new OrganizationV3DirectoriesPatchRequest(directoryId, active, androidKey, iosP12, denialContextInquiryEnabled, webhookUrl);
            _transport.OrganizationV3DirectoriesPatch(request, _organizationId);
        }

        public Directory GetDirectory(Guid directoryId)
        {
            return GetDirectories(new List<Guid> { directoryId })[0];
        }

        public List<Directory> GetDirectories(List<Guid> directoryIds)
        {
            var request = new OrganizationV3DirectoriesListPostRequest(directoryIds);
            var response = _transport.OrganizationV3DirectoriesListPost(request, _organizationId);
            var directories = new List<Directory>();

            foreach (OrganizationV3DirectoriesListPostResponse.Directory directoryItem in response.Directories)
            {
                directories.Add(new Directory(
                    directoryItem.Id,
                    directoryItem.Name,
                    directoryItem.Active,
                    directoryItem.ServiceIds,
                    directoryItem.SdkKeys,
                    directoryItem.AndroidKey,
                    directoryItem.IosCertificateFingerprint,
                    directoryItem.DenialContextInquiryEnabled,
                    directoryItem.WebhookUrl
                ));
            }

            return directories;
        }

        public List<Directory> GetAllDirectories()
        {
            var response = _transport.OrganizationV3DirectoriesGet(_organizationId);
            var directories = new List<Directory>();

            foreach (OrganizationV3DirectoriesGetResponse.Directory directoryItem in response.Directories)
            {
                directories.Add(new Directory(
                    directoryItem.Id,
                    directoryItem.Name,
                    directoryItem.Active,
                    directoryItem.ServiceIds,
                    directoryItem.SdkKeys,
                    directoryItem.AndroidKey,
                    directoryItem.IosCertificateFingerprint,
                    directoryItem.DenialContextInquiryEnabled,
                    directoryItem.WebhookUrl
                ));
            }

            return directories;
        }

        public Guid GenerateAndAddDirectorySdkKey(Guid directoryId)
        {
            var response = _transport.OrganizationV3DirectorySdkKeysPost(new OrganizationV3DirectorySdkKeysPostRequest(directoryId), _organizationId);
            return response.SdkKey;
        }

        public void RemoveDirectorySdkKey(Guid directoryId, Guid sdkKey)
        {
            _transport.OrganizationV3DirectorySdkKeysDelete(new OrganizationV3DirectorySdkKeysDeleteRequest(directoryId, sdkKey), _organizationId);
        }

        public List<Guid> GetAllDirectorySdkKeys(Guid directoryId)
        {
            var request = new OrganizationV3DirectorySdkKeysListPostRequest(directoryId);
            var response = _transport.OrganizationV3DirectorySdkKeysListPost(request, _organizationId);
            return response.SdkKeys;
        }

        public List<PublicKey> GetServicePublicKeys(Guid serviceId)
        {
            var request = new ServiceKeysListPostRequest(serviceId);
            var response = _transport.OrganizationV3ServiceKeysListPost(request, _organizationId);
            return response.FromTransport();
        }

        public string AddServicePublicKey(Guid serviceId, string publicKeyPem, bool active, DateTime? expires, KeyType keyType = KeyType.BOTH)
        {
            var request = new ServiceKeysPostRequest(
                serviceId,
                publicKeyPem,
                expires?.ToUniversalTime(),
                active,
                (int)keyType
            );
            var response = _transport.OrganizationV3ServiceKeysPost(request, _organizationId);
            return response.Id;
        }

        public string AddServicePublicKey(Guid serviceId, string publicKeyPem, bool active, DateTime? expires)
        {
            return AddServicePublicKey(serviceId, publicKeyPem, active, expires, KeyType.BOTH);
        }

        public void UpdateServicePublicKey(Guid serviceId, string keyId, bool active, DateTime? expires)
        {
            var request = new ServiceKeysPatchRequest(
                serviceId,
                keyId,
                expires?.ToUniversalTime(),
                active
            );
            _transport.OrganizationV3ServiceKeysPatch(request, _organizationId);
        }

        public void RemoveServicePublicKey(Guid serviceId, string keyId)
        {
            var request = new ServiceKeysDeleteRequest(serviceId, keyId);
            _transport.OrganizationV3ServiceKeysDelete(request, _organizationId);
        }

        [Obsolete("GetServicePolicy is deprecated, please use GetAdvancedServicePolicy instead")]
        public ServicePolicy GetServicePolicy(Guid serviceId)
        {
            Domain.Service.Policy.IPolicy legacyPolicy = GetAdvancedServicePolicy(serviceId);

            if (legacyPolicy.GetType() != typeof(LegacyPolicy))
            {
                Trace.TraceWarning($"Invalid policy type returned to legacy function. To utilize new policies please use GetAdvancedServicePolicy");
                return null;
            }

            // This calls ToTransport because the parsing logic that is contained in the ServicePolicy class shouldn't be duplicated
            return ServicePolicy.FromTransport((Transport.Domain.AuthPolicy)legacyPolicy.ToTransport());
        }

        [Obsolete("SetServicePolicy is deprecated, please use SetAdvancedServicePolicy instead")]
        public void SetServicePolicy(Guid serviceId, ServicePolicy policy)
        {
            SetAdvancedServicePolicy(serviceId, policy.ToLegacyPolicy());
        }

        public Domain.Service.Policy.IPolicy GetAdvancedServicePolicy(Guid serviceId)
        {
            var request = new ServicePolicyItemPostRequest(serviceId);
            Transport.Domain.IPolicy response = _transport.OrganizationV3ServicePolicyItemPost(request, _organizationId);
            return response.FromTransport();
        }

        public void SetAdvancedServicePolicy(Guid serviceId, Domain.Service.Policy.IPolicy policy)
        {
            var request = new ServicePolicyPutRequest(serviceId, policy.ToTransport());
            _transport.OrganizationV3ServicePolicyPut(request, _organizationId);
        }

        public void RemoveServicePolicy(Guid serviceId)
        {
            var request = new ServicePolicyDeleteRequest(serviceId);
            _transport.OrganizationV3ServicePolicyDelete(request, _organizationId);
        }

        public List<PublicKey> GetDirectoryPublicKeys(Guid directoryId)
        {
            var request = new DirectoryKeysListPostRequest(directoryId);
            var response = _transport.OrganizationV3DirectoryKeysListPost(request, _organizationId);
            var keys = new List<PublicKey>();
            return response.FromTransport();
        }

        public string AddDirectoryPublicKey(Guid directoryId, string publicKeyPem, bool active, DateTime? expires, KeyType keyType = KeyType.BOTH)
        {
            var request = new DirectoryKeysPostRequest(
                directoryId,
                publicKeyPem,
                expires?.ToUniversalTime(),
                active,
                (int)keyType
            );
            var response = _transport.OrganizationV3DirectoryKeysPost(request, _organizationId);
            return response.Id;
        }

        public string AddDirectoryPublicKey(Guid directoryId, string publicKeyPem, bool active, DateTime? expires)
        {
            return AddDirectoryPublicKey(directoryId, publicKeyPem, active, expires, KeyType.BOTH);
        }

        public void UpdateDirectoryPublicKey(Guid directoryId, string keyId, bool active, DateTime? expires)
        {
            var request = new DirectoryKeysPatchRequest(
                directoryId,
                keyId,
                expires?.ToUniversalTime(),
                active
            );
            _transport.OrganizationV3DirectoryKeysPatch(request, _organizationId);
        }

        public void RemoveDirectoryPublicKey(Guid directoryId, string keyId)
        {
            var request = new DirectoryKeysDeleteRequest(directoryId, keyId);
            _transport.OrganizationV3DirectoryKeysDelete(request, _organizationId);
        }
    }
}
