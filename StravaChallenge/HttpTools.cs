using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace StravaChallenge
{
    static internal class HttpTools
    {
        public static async Task<T> GetHttpResponseAsync<T>(string requestUri, string accessToken)
        {
            var client = GetJsonHttpClient();
            if (accessToken != null)
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            var response = await client.GetAsync(requestUri);
            var results = await HttpContentExtensions.ReadAsAsync<T>(response.Content);
            return results;
        }

        public static HttpClient GetJsonHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}