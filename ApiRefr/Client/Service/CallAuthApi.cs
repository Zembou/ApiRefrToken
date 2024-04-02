using ApiRefr.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using RestSharp;
using Azure;
using ApiRefr;
using Azure.Core;
using System.Net.Http.Headers;
using ApiRefr.Class;

namespace Client.Service
{
    public class CallAuthApi
    {
        public HttpResponse _Context { get; set; }
        public CallAuthApi()
        {
            
        }
        
        public async Task<TokenApiModel> Auth(LoginModel Login)
        {
            HttpClient client = new HttpClient();
            
           
            var json = JsonConvert.SerializeObject(Login);
            var content = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            "https://localhost:7099/api/Auth/Login")
            {
                Headers =
                {
                    { HeaderNames.Accept, "application/json" },
                    { HeaderNames.WWWAuthenticate, "Negociate" },
                },
                Content = content
            };
            var response = await client.SendAsync(httpRequestMessage);

            var value =  await response.Content.ReadFromJsonAsync<TokenApiModel>();
            Console.WriteLine(value);
            var cleanValue = JsonConvert.SerializeObject(value);
            Console.WriteLine(cleanValue);
            var objectValue = JsonConvert.DeserializeObject<TokenApiModel>(cleanValue);
            Console.WriteLine(objectValue);
            return objectValue;
            
        }

        public async Task<IEnumerator<WeatherForecast>> Forecast(TokenApiModel token)
        {
            HttpClient client = new HttpClient();

            var accessToken = token.AccessToken;
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            "https://localhost:7099/api/Auth/Login")
            {
                Headers =
                {
                    { HeaderNames.Accept, "application/json" },
                    { HeaderNames.WWWAuthenticate, "Negociate" },
                    { HeaderNames.Authorization, new AuthenticationHeaderValue("Bearer", accessToken).ToString() },
                },
            };
            var response = await client.SendAsync(httpRequestMessage);

            //var value = response.Content;
            var value = await response.Content.ReadFromJsonAsync<IEnumerator<WeatherForecast>>();
            Console.WriteLine(value);
            var cleanValue = JsonConvert.SerializeObject(value);
            Console.WriteLine(cleanValue);
            var objectValue = JsonConvert.DeserializeObject<IEnumerator<WeatherForecast>>(cleanValue);
            Console.WriteLine(objectValue);

            return objectValue;

        }
    }
}
