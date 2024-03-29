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

namespace Client.Service
{
    public class CallAuthApi
    {
        public async Task<IActionResult> Auth(LoginModel Login)
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

            string value =  await response.Content.ReadAsStringAsync();
            Console.WriteLine(value + "Ceci est le deuxieme writeline");
            HttpContext.Response.Cookies.Append("AccessToken", value,
    new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTime.Now.AddMinutes(expires_in) });

            return new OkResult();
            
        }
    }
}
