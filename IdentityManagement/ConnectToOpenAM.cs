using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http.Formatting;
using IdentityManagement.Dto;
using IdentityManagement.Models;
using IdentityManagement.OpenAM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdentityManagement
{
    public class ConnectToOpenAM
    {
        private readonly HttpClient httpClient;
        private string _host;
        
        private const string TokenEndpoint = "/oauth2/access_token";
        private const string TokenInfoEndpoint = "/oauth2/tokeninfo";
        private string host;
        private OpenAMDetails details;

        public ConnectToOpenAM(string host)
        {
            httpClient = new HttpClient();
            this._host = host;
        }

        public ConnectToOpenAM(HttpClient httpClient, string host)
        {
            // TODO: Complete member initialization
            this.httpClient = httpClient;
            this._host = host;
        }

        public ConnectToOpenAM(HttpClient httpClient, OpenAMDetails details)
        {
            // TODO: Complete member initialization
            this.httpClient = httpClient;
            this._host = details.Host;
            this.details = details;
        }
        public ConnectToOpenAM(OpenAMDetails details): this(details.Host)
        {
            this.details = details;
        }
        

        public async Task<OpenAMAccessTokenInfo> GetTokenInfo(string accessToken)
        {
            var userResponse = await GetJson(HttpMethod.Get, TokenInfoEndpoint + "?access_token=" + Uri.EscapeDataString(accessToken));
            var openAMAccessTokenInfo = JsonConvert.DeserializeObject<OpenAMAccessTokenInfo>(userResponse);
            return openAMAccessTokenInfo;
        }

        

        private async Task<string> GetJson(HttpMethod httpMethod, string path)
        {
            return await GetJson(httpMethod, path, null);
        }

        private async Task<string> GetJson(HttpMethod httpMethod, string path, IEnumerable<KeyValuePair<string, string>> body)
        {
            var requestMessage = new HttpRequestMessage(httpMethod, GetServerPath(path));
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (body != null) requestMessage.Content = new FormUrlEncodedContent(body);
            var response = await httpClient.SendAsync(requestMessage);
            return await response.Content.ReadAsStringAsync();

        }
        public async Task<OpenAMAccessToken> SwapCodeForToken(string code, string redirectUri)
        {
            var body = GetTokenRequestBody(code, redirectUri);
            return await GetOpenAMAccessToken(body);
        }

        public async Task<OpenAMAccessToken> GetOAuthToken(string username, string password, string scope)
        {
            var body = GetDirectTokenRequestBody(username, password, scope);
            return await GetOpenAMAccessToken(body);
        }

        private static string BodyScope(string[] scope)
        {
            return FlattenScope(scope, " ");
        }

        private async Task<OpenAMAccessToken> GetOpenAMAccessToken(IEnumerable<KeyValuePair<string, string>> body)
        {
            var tokenJson = await GetJson(HttpMethod.Post, TokenEndpoint, body);
            var token = JsonConvert.DeserializeObject<OpenAMAccessToken>(tokenJson);
            return token;
        }


        public async Task<OpenAMAccessToken> RefreshToken(string refreshToken, string scope)
        {
            var body = GetTokenRefreshBody(refreshToken,  scope);
            return await GetOpenAMAccessToken(body);
        }

        private IEnumerable<KeyValuePair<string, string>> GetTokenRefreshBody(string refreshToken, string scope)
        {            
            var body = BaseRequestBody("refresh_token");
            body.Add(new KeyValuePair<string, string>("refresh_token", refreshToken));
            body.Add(GetScopeItem(scope));
            return body;
        }

       

        private IEnumerable<KeyValuePair<string, string>> GetTokenRequestBody(string code, string redirectUri)
        {
            // Build up the body for the token request
            var body = BaseRequestBody("authorization_code");
            body.Add(new KeyValuePair<string, string>("code", code));
            body.Add(new KeyValuePair<string, string>("redirect_uri", redirectUri));
            return body;
        }


        

        private List<KeyValuePair<string, string>> BaseRequestBody(string grantType)
        {
            var body = new List<KeyValuePair<string, string>>();
            body.Add(new KeyValuePair<string, string>("client_id", details.ClientId));
            body.Add(new KeyValuePair<string, string>("client_secret", details.ClientSecret));
            body.Add(new KeyValuePair<string, string>("grant_type", grantType));
            return body;
        }

        private string GetServerPath(string path)
        {
            return _host + path;
        }

        public string GetStandardToken(string username, string password)
        {
            var target = String.Format("{0}/identity/authenticate?username={1}&password={2}", _host, username, password);
            var response = GetHttpResponse(target);
            var returnedToken = response.Split('=')[1];
            var trimmedToken = TrimNewLineFromEnd(returnedToken);
            return trimmedToken;
        }

        private static string TrimNewLineFromEnd(string returnedToken)
        {
            return returnedToken.TrimEnd(Environment.NewLine.ToCharArray());
        }

        public static string GetHttpResponse(string targetUri)
        {
            return GetHttpResponseAsync(targetUri).Result;
        }

        public static T GetHttpObject<T>(string targetUri)
        {
            return GetHttpObjectAsync<T>(targetUri).Result;
        }

        public static async Task<T> GetHttpObjectAsync<T>(string targetUri)
        {
            HttpResponseMessage httpReponse;
            using (var client = new HttpClient())
                httpReponse = await client.GetAsync(targetUri);
            return await httpReponse.Content.ReadAsAsync<T>();
        }

        public static async Task<string> GetHttpResponseAsync(string targetUri)
        {
            HttpResponseMessage httpReponse;
            using (var client = new HttpClient())
                httpReponse = await client.GetAsync(targetUri);
            return await httpReponse.Content.ReadAsStringAsync();
        }

        public Identity GetUserFromStandardToken(string token)
        {
            var target = String.Format("{0}/identity/attributes?subjectid={1}", _host, token);
            var response = GetHttpResponse(target);
            var identity = Mapping.MapPropertyList(response);
            return identity;
        }

        public bool CheckToken(string tokenToCheck)
        {
            var target = String.Format("{0}/identity/isTokenValid?tokenid={1}", _host, tokenToCheck);
            var response = GetHttpResponse(target);
            return response.Contains("true");
        }

       

        private IEnumerable<KeyValuePair<string, string>> GetDirectTokenRequestBody(string username, string password, string scope)
        {
            // Build up the body for the token request
            var body = BaseRequestBody("password");
            body.Add(new KeyValuePair<string, string>("username", username));
            body.Add(new KeyValuePair<string, string>("password", password));
            body.Add(GetScopeItem(scope));
            return body;
        }

        private static KeyValuePair<string, string> GetScopeItem(string flattenedScope)
        {
            
            return new KeyValuePair<string, string>("scope", flattenedScope);
        }

        public static string FlattenScope(IEnumerable<string> scope, string delim)
        {
            return String.Join(delim, scope);
        }
    }
}
