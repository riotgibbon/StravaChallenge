// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Security.Claims;
using IdentityManagement.Dto;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;

namespace IdentityManagement.OpenAM.Provider
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class OpenAMAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="OpenAMAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized user</param>
        /// <param name="refreshToken">OpenAM Access token</param>
        public OpenAMAuthenticatedContext(IOwinContext context, OpenAMAccessTokenInfo user, string refreshToken)
            : base(context)
        {
            User = user;
            AccessToken = user.access_token; ;
            //TODO - update details
            Id = user.uid;
            Name = user.cn;
            Email = user.mail;
            RefreshToken = refreshToken;
        }

        /// <summary>
        /// Gets the JSON-serialized user
        /// </summary>
        /// <remarks>
        /// Contains the OpenAM user obtained from the endpoint https://api.OpenAM.com/user
        /// </remarks>
        public OpenAMAccessTokenInfo User { get; private set; }

        /// <summary>
        /// Gets the OpenAM access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the OpenAM user ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the user's name
        /// </summary>
        public string Name { get; private set; }

        public string Email { get; private set; }

        /// <summary>
        /// Gets the OpenAM username
        /// </summary>
        public string RefreshToken { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }

        private static string TryGetValue(JObject user, string propertyName)
        {
            JToken value;
            return user.TryGetValue(propertyName, out value) ? value.ToString() : null;
        }
    }
}
