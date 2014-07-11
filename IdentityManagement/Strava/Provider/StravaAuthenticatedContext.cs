// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Security.Claims;
using IdentityManagement.Dto;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;

namespace IdentityManagement.Strava.Provider
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class StravaAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="StravaAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized user</param>
        /// <param name="refreshToken">Strava Access token</param>
        public StravaAuthenticatedContext(IOwinContext context, StravaAccessToken user)
            : base(context)
        {
            User = user;
            AccessToken = user.access_token; ;
            //TODO - update details
            Id = user.athlete.id.ToString();
            Name = user.athlete.firstname + " " + user.athlete.lastname;
            Email = user.athlete.email;

        }

        /// <summary>
        /// Gets the JSON-serialized user
        /// </summary>
        /// <remarks>
        /// Contains the Strava user obtained from the endpoint https://api.Strava.com/user
        /// </remarks>
        public StravaAccessToken User { get; private set; }

        /// <summary>
        /// Gets the Strava access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the Strava user ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the user's name
        /// </summary>
        public string Name { get; private set; }

        public string Email { get; private set; }

        /// <summary>
        /// Gets the Strava username
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
