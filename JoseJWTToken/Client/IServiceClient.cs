﻿using JoseJWTToken.Domain.Service;
using System.Collections.Generic;

namespace JoseJWTToken.Client
{
    public interface IServiceClient : IWebhookHandler
    {
        /// <summary>
        /// Perform an authorization for a user of a service.
        /// </summary>
        /// <param name="user">The username or directory user ID to authorize</param>
        /// <param name="context">The message to display on the device during authorization request</param>
        /// <param name="policy">The authorization policy to use when authorizing the user</param>
        /// <returns>The authorization request identifier, which can be used for checking the status of this request</returns>
        [System.Obsolete("Authorize is deprecated in favor of CreateAuthorizationRequest")]
        string Authorize(string user, string context = null, AuthPolicy policy = null);

        /// <summary>
        /// Create an authorization request for a user of a service.
        /// </summary>
        /// <param name="user">The username or directory user ID to authorize</param>
        /// <param name="context">The message to display on the device during authorization request</param>
        /// <param name="policy">The authorization policy to use when authorizing the user</param>
        /// <param name="title">The title to display on the device during authorization request</param>
        /// <param name="ttl">Time to live in seconds for the authorization request. If not provided or null, the system default will be used</param>
        /// <returns>Information regarding the authorization request.</returns>
        AuthorizationRequest CreateAuthorizationRequest(string user, string context = null, AuthPolicy policy = null, string title = null, int? ttl = null, string pushTitle = null, string pushBody = null, IList<DenialReason> denialReasons = null);

        /// <summary>
        /// Cancels the authorization request.
        /// </summary>
        /// <param name="authorizationRequestId">The authorization request identifier, usually retrieved via CreateAuthorizationRequest()</param>
        void CancelAuthorizationRequest(string authorizationRequestId);

        /// <summary>
        /// Retrieve the status of an authorization request.
        /// </summary>
        /// <param name="authorizationRequestId">The authorization request identifier, usually retrieved via CreateAuthorizationRequest()</param>
        /// <returns>NULL if the authorization is pending. A response package once the user has responded.</returns>
        AuthorizationResponse GetAuthorizationResponse(string authorizationRequestId);

        /// <summary>
        /// Retrieve the status of an Advanced authorization request.
        /// </summary>
        /// <param name="authorizationRequestId">The authorization request identifier, usually retrieved via CreateAuthorizationRequest()</param>
        /// <returns>NULL if the authorization is pending. A response package once the user has responded.</returns>
        AdvancedAuthorizationResponse GetAdvancedAuthorizationResponse(string authorizationRequestId);

        /// <summary>
        /// Begin a session for a user of this service.
        /// </summary>
        /// <param name="user">The username or directory user ID</param>
        void SessionStart(string user);

        /// <summary>
        /// Begin a session for a user of this service, tied to a specific authorization request.
        /// </summary>
        /// <param name="user">The username or directory user ID</param>
        /// <param name="authorizationRequestId">the authorization request that is associated with this session</param>
        void SessionStart(string user, string authorizationRequestId);

        /// <summary>
        /// Ends a session for a user of this service.
        /// </summary>
        /// <param name="user">The username or directory user ID to end the session for</param>
        void SessionEnd(string user);
        
        /// <summary>
        /// Verifies a given TOTP is valid for a given user.
        /// </summary>
        /// <param name="user">LaunchKey Username, User Push ID, or Directory User ID for the End User</param>
        /// <param name="otp">6-8 digit OTP code for verification</param>
        bool VerifyTotp(string user, string otp);
    }
}