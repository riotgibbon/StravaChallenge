using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityManagement.Dto;
using IdentityManagement.Strava.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdentityManagement.Strava
{
    public class StravaAuthenticationHandler : AuthenticationHandler<StravaAuthenticationOptions>
    {
        private const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";
        private const string AuthorizeEndpoint = "/oauth/authorize";

        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        

        public StravaAuthenticationHandler(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationProperties properties = null;

            try
            {
                string code = null;
                string state = null;
                var details = new StravaDetails
                {
                    ClientId = Options.ClientId,
                    ClientSecret = Options.ClientSecret,
                    Host = Options.Host
                };
                var connectToStrava = new ConnectToStrava(this.httpClient, details);
                IReadableStringCollection query = Request.Query;
                IList<string> values = query.GetValues("code");
                if (values != null && values.Count == 1)
                {
                    code = values[0];
                }
                values = query.GetValues("state");
                if (values != null && values.Count == 1)
                {
                    state = values[0];
                }

                //properties = Options.StateDataFormat.Unprotect(state);
                //if (properties == null)
                //{
                //    return null;
                //}

                //// OAuth2 10.12 CSRF
                //if (!ValidateCorrelationId(properties, logger))
                //{
                //    return new AuthenticationTicket(null, properties);
                //}

                string requestPrefix = Request.Scheme + "://" + Request.Host;
                string redirectUri = requestPrefix + Request.PathBase + Options.CallbackPath;

                ;
                var tokenResponse = await connectToStrava.SwapCodeForToken(code,redirectUri);

                //var user = await connectToStrava.GetTokenInfo(tokenResponse.access_token);

                var context = new StravaAuthenticatedContext(Context, tokenResponse)
                {
                    Identity = new ClaimsIdentity(
                        Options.AuthenticationType,
                        ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType)
                };

                AddClaimToContext(context, context.Id, ClaimTypes.NameIdentifier);
                AddClaimToContext(context, context.Name, ClaimsIdentity.DefaultNameClaimType);
                AddClaimToContext(context, context.AccessToken, "urn:tokens:accessToken");
                //AddClaimToContext(context, context.RefreshToken, "urn:tokens:refreshToken");
                AddClaimToContext(context, context.Email, ClaimTypes.Email);
               
                properties  = new AuthenticationProperties();
                properties.RedirectUri = requestPrefix + Request.PathBase + "/Account/ExternalLoginCallback";
                context.Properties = properties;

                await Options.Provider.Authenticated(context);

                return new AuthenticationTicket(context.Identity, context.Properties);
            }
            catch (Exception ex)
            {
                logger.WriteError(ex.Message);
            }
            return new AuthenticationTicket(null, properties);
        }


        private void AddClaimToContext(StravaAuthenticatedContext context, string claimValue, string claimIdentifier)
        {
            if (!string.IsNullOrEmpty(claimValue))
            {
                context.Identity.AddClaim(new Claim(claimIdentifier, claimValue, XmlSchemaString, Options.AuthenticationType));
            }
        }

        private string GetServerPath(string path)
        {
            return Options.Host + path;
        }
        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
            {
                return Task.FromResult<object>(null);
            }

            AuthenticationResponseChallenge challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

            if (challenge != null)
            {
                string baseUri =
                    Request.Scheme +
                    Uri.SchemeDelimiter +
                    Request.Host +
                    Request.PathBase;

                string currentUri =
                    baseUri +
                    Request.Path +
                    Request.QueryString;

                string redirectUri =
                    baseUri +
                    Options.CallbackPath;

                AuthenticationProperties properties = challenge.Properties;
                if (string.IsNullOrEmpty(properties.RedirectUri))
                {
                    properties.RedirectUri = currentUri;
                }

                // OAuth2 10.12 CSRF
                GenerateCorrelationId(properties);

                // comma separated
                //string scope = ConnectToStrava.FlattenScope(Options.Scope, " ");
                string scope = Options.Scope;

                string state = Options.StateDataFormat.Protect(properties);

                string authorizationEndpoint =
                    //"https://www.strava.com/oauth/authorize?client_id=2159&response_type=code&redirect_uri=http://stowstdf.azurewebsites.net&approval_prompt=force";
                    GetServerPath(AuthorizeEndpoint) +
                        "?client_id=" + Uri.EscapeDataString(Options.ClientId) +
                         "&response_type=code" +
                     "&redirect_uri="     + Uri.EscapeDataString(redirectUri) +
                     ///   "&redirect_uri=" + // + Uri.EscapeDataString("http://stowstdf.azurewebsites.net") +
                        "&approval_prompt=force"
                    //"&scope=" + Uri.EscapeDataString(scope) +
                    //"&state=" + Uri.EscapeDataString(state)
                        ;

                Response.Redirect(authorizationEndpoint);
            }

            return Task.FromResult<object>(null);
        }

        public override async Task<bool> InvokeAsync()
        {
            return await InvokeReplyPathAsync();
        }

        private async Task<bool> InvokeReplyPathAsync()
        {
            if (Options.CallbackPath.HasValue && Options.CallbackPath == Request.Path)
            {
                // TODO: error responses

                AuthenticationTicket ticket = await AuthenticateAsync();
                if (ticket == null)
                {
                    logger.WriteWarning("Invalid return state, unable to redirect.");
                    Response.StatusCode = 500;
                    return true;
                }

                var context = new StravaReturnEndpointContext(Context, ticket);
                context.SignInAsAuthenticationType = Options.SignInAsAuthenticationType;
                context.RedirectUri = ticket.Properties.RedirectUri;

                await Options.Provider.ReturnEndpoint(context);

                if (context.SignInAsAuthenticationType != null &&
                    context.Identity != null)
                {
                    ClaimsIdentity grantIdentity = context.Identity;
                    if (!string.Equals(grantIdentity.AuthenticationType, context.SignInAsAuthenticationType, StringComparison.Ordinal))
                    {
                        grantIdentity = new ClaimsIdentity(grantIdentity.Claims, context.SignInAsAuthenticationType, grantIdentity.NameClaimType, grantIdentity.RoleClaimType);
                    }
                    Context.Authentication.SignIn(context.Properties, grantIdentity);
                }

                if (!context.IsRequestCompleted && context.RedirectUri != null)
                {
                    string redirectUri = context.RedirectUri;
                    if (context.Identity == null)
                    {
                        // add a redirect hint that sign-in failed in some way
                        redirectUri = WebUtilities.AddQueryString(redirectUri, "error", "access_denied");
                    }
                    Response.Redirect(redirectUri);
                    context.RequestCompleted();
                }

                return context.IsRequestCompleted;
            }
            return false;
        }
    }

    public class StravaDetails
    {
        public string Host { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}