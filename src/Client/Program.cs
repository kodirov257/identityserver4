using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // discover endpoint from metadata
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError)
            {
                Console.WriteLine(DownloadStringCompletedEventArgs.Error);
                return;
            }
            
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                
                ClientId = "uztelecom",
                ClientSecret = "VMx3HfE62Ju82CF",
                Scope = "auth",
            });
            
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }
            
            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");
            
            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("https://localhost:7233/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}
