using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Quisy.Tools.HttpHelpers
{
    public class HttpClientHelper
    {
        public static async Task<T> Get<T>(string url, AuthenticationHeaderValue authHeader = null)
        {
            try
            {
                var response = await CallClient(url, authHeader);
                return response.IsSuccessStatusCode ? 
                    DeserializeContent<T>(response.Content) :
                    default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling url: {url}. Exception: {ex.Message}");
                return default;
            }
        }        

        private static T DeserializeContent<T>(HttpContent content)
        {
            var responseString = content.ReadAsStringAsync().Result;
            var apiResponseObject = JsonConvert.DeserializeObject<T>(responseString);
            return apiResponseObject;
        }

        private static async Task<HttpResponseMessage> CallClient(string url, AuthenticationHeaderValue authHeader = null)
        {
            using (var client = new HttpClient())
            {
                if(authHeader != null)
                    client.DefaultRequestHeaders.Authorization = authHeader;
                return await client.GetAsync(url);
            }
        }
    }
}
