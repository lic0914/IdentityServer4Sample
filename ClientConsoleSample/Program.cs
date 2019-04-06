using System;
using System.Net.Http;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace ClientConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var http = new HttpClient();
            var disc= http.GetDiscoveryDocumentAsync("http://localhost:5000").Result;
            var tokenResponse =  http.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disc.TokenEndpoint,

                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            }).Result;
            Console.WriteLine(tokenResponse.Json);
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            var response =  client.GetAsync("http://localhost:5001/identity").Result;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content =  response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(JArray.Parse(content));
            }
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
