using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SWH.Api.Contracts
{
    public class SmartWaterApi : ISmartWaterApi
    {
        private readonly string _baseUrl;

        public SmartWaterApi(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        private HttpClient GetClient(string token)
        {
            return new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                DefaultRequestHeaders =
                {
                    Authorization = string.IsNullOrWhiteSpace(token)
                        ? null
                        : new AuthenticationHeaderValue("Bearer", token)
                }
            };
        }

        public async Task PostSensorReport(string token, SmartSensorReport report)
        {
            const string actionUrl = "/sensor/report";

            using (var clt = GetClient(token))
            {
                var response = await clt.PostAsync(actionUrl, new StringContent(JsonConvert.SerializeObject(report), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: error handling?
                }
            }
        }
    }
}