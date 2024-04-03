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
using Microsoft.AspNetCore.Mvc.Filters;

namespace Client.Service
{
    public class CallAuthApi
    {
        public IHttpContextAccessor _context { get; set; }
        public CallAuthApi(IHttpContextAccessor context)
        {
            _context = context;
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
            
           

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                try
                {
                    var value = await response.Content.ReadFromJsonAsync<TokenApiModel>();
                    var cleanValue = JsonConvert.SerializeObject(value);
                    var objectValue = JsonConvert.DeserializeObject<TokenApiModel>(cleanValue);
                    return objectValue;
                }
                catch
                {
                    return null;
                }

            }
            
            
        }

        public async Task<IEnumerable<WeatherForecast>> Forecast(TokenApiModel token /* , HttpContext context*/)
        {
            HttpClient client = new HttpClient();

            var accessToken = token.AccessToken;

            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            "https://localhost:7099/WeatherForecast")
            {
                Headers =
                {
                    { HeaderNames.Accept, "application/json" },
                    { HeaderNames.WWWAuthenticate, "Negociate" },
                    { HeaderNames.Authorization, new AuthenticationHeaderValue("Bearer", accessToken).ToString() },
                },
            };
            var response = await client.SendAsync(httpRequestMessage);

            try
            {
                var value = await response.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();
                var cleanValue = JsonConvert.SerializeObject(value);
                var objectValue = JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(cleanValue);

                return objectValue;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> IsTokenValid(string token)
        {
            HttpClient client = new HttpClient();

            var json = JsonConvert.SerializeObject(token);

            var content = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            "https://localhost:7099/api/Token/checkTokenTime")
            {
                Headers =
                {
                    { HeaderNames.Accept, "application/json" },
                    { HeaderNames.WWWAuthenticate, "Negociate" },
                },
                Content = content
            };
            try
            {
                var response = await client.SendAsync(httpRequestMessage);

                var value = await response.Content.ReadFromJsonAsync<bool>();
                var cleanValue = JsonConvert.SerializeObject(value);
                var objectValue = JsonConvert.DeserializeObject<bool>(cleanValue);

                return objectValue;
            }
            catch
            {
                return false;
            }
            
        }

        public async Task<TokenApiModel> RegenerateRefreshToken(HttpContext context)
        {
            HttpClient client = new HttpClient();
            var token = new TokenApiModel()
            {
                AccessToken = context.Request.Cookies["AccessToken"],
                RefreshToken = context.Request.Cookies["RefreshToken"]
            };

            var json = JsonConvert.SerializeObject(token);
            var content = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            "https://localhost:7099/api/Token/Refresh")
            {
                Headers =
                {
                    { HeaderNames.Accept, "application/json" },
                    { HeaderNames.WWWAuthenticate, "Negociate" },
                },
                Content = content
            };
            var response = await client.SendAsync(httpRequestMessage);
            var value = await response.Content.ReadFromJsonAsync<TokenApiModel>();
            if (value == null)
            {
                return new TokenApiModel();
            }
            else
            {
                try
                {
                    var cleanValue = JsonConvert.SerializeObject(value);
                    var objectValue = JsonConvert.DeserializeObject<TokenApiModel>(cleanValue);
                    return objectValue;
                }
                catch
                {
                    return new TokenApiModel();
                }

            }

        }

        public async Task<TokenApiModel> ValidateToken(HttpContext context)
        {
            if (await IsTokenValid(context.Request.Cookies["AccessToken"]))
            {
                return new TokenApiModel()
                {
                    AccessToken = context.Request.Cookies["AccessToken"],
                    RefreshToken = context.Request.Cookies["RefreshToken"]
                };
            } else
            {
                return await RegenerateRefreshToken(context);
            }
        }
    }
}
