using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Identity.Client
{
    class Program
    {
        private static string serverBaseAddress = "http://localhost:5000";
        private static string apiBaseAddress = "http://localhost:5002";

        /// <summary>
        /// 本示例包含两种演示方式请求授权服务器
        /// 1、通过引入IdentityModel包
        /// 2、通过HttpClient
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            var serverClient = new HttpClient { BaseAddress = new Uri(serverBaseAddress) };

            var flag = true;
            switch (flag)
            {
                case true:
                    await UseIdentityModelRequest(serverClient, RequestApiResource);
                    break;
                default:
                    await UseHttpClientRequest(serverClient, RequestApiResource);
                    break;
            }

            Console.ReadLine();
        }

        /// <summary>
        /// 通过IDS4自带的IdentityModel包请求授权服务
        /// </summary>
        /// <param name="serverClient"></param>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        private static async Task UseIdentityModelRequest(HttpClient serverClient, Action<string> apiRequest)
        {
            var disco = await serverClient.GetDiscoveryDocumentAsync();
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);

                Console.Read();
                return;
            }

            var tokenResponse = await serverClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client_api_001",
                ClientSecret = "secret",
                //Scope = "api"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);

                Console.Read();
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call Identity Resource API
            RequestApiResource(tokenResponse.AccessToken);
        }

        /// <summary>
        /// 使用纯HttpClient请求授权服务
        /// </summary>
        /// <param name="serverClient"></param>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        private static async Task UseHttpClientRequest(HttpClient serverClient, Action<string> apiRequest)
        {
            var dicts = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = "client_api_001",
                ["client_secret"] = "secret"
            };
            var response = await serverClient.PostAsync("connect/token", new FormUrlEncodedContent(dicts));
            var content = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<dynamic>(content);

            Console.WriteLine(obj);
            Console.WriteLine("\n\n");

            apiRequest(Convert.ToString(obj.access_token));
        }

        /// <summary>
        /// 根据提供的 <c>AccessToken</c> 请求Api资源。
        /// </summary>
        /// <param name="accessToken"></param>
        private static async void RequestApiResource(string accessToken)
        {
            var apiClient = new HttpClient
            {
                BaseAddress = new Uri(apiBaseAddress)
            };
            apiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var result = await apiClient.GetStringAsync("api/identity");

            Console.WriteLine(JArray.Parse(result));
        }
    }
}
