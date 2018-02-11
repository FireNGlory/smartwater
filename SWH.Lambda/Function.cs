using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function"s JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SWH.Lambda
{
    public class DeviceDefinition
    {
        public string applianceId { get; set; }
        public string manufacturerName { get; set; }
        public string modelName { get; set; }
        public string version { get; set; }
        public string friendlyName { get; set; }
        public string friendlyDescription { get; set; }
        public bool isReachable { get; set; }
        public ICollection<string> actions { get; set; }
    }

    public class Function
    {
public dynamic FunctionHandler(dynamic input, ILambdaContext context)
{
    // check what type of a request it is like an IntentRequest or a LaunchRequest
    log("input", JsonConvert.SerializeObject(input));
    log("context", JsonConvert.SerializeObject(context));

    HttpResponseMessage resp;
            
    var clt = new HttpClient();

    string token = "";
    string command = input.directive.header.name;

    switch (command.ToLower())
    {
        case "discover":
            token = input.directive.payload.scope.token;
            break;
        case "reportstate":
            token = input.directive.endpoint.scope.token;
            break;
    }
            
    clt.BaseAddress = new Uri("https://smartwater.azurewebsites.net");
    clt.DefaultRequestHeaders.Accept.Clear();
    clt.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    clt.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);
            
    resp =
        clt.PostAsync($"api/smarthome/{command}/{context.AwsRequestId}",
        new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json")).Result;

    if (!resp.IsSuccessStatusCode)
    {
        clt.Dispose();
        return GenError(resp.StatusCode.ToString());
    }

    var jsonResp =  resp.Content.ReadAsStringAsync().Result;

    clt.Dispose();

    log("success", command);
    log("response", jsonResp);

    return JsonConvert.DeserializeObject<dynamic>(jsonResp);
} /**
         * Utility functions
         */

        private dynamic GenError(string message)
        {
            log("ERROR", message);
            return null;
        }

        private void log(string title, string msg)
        {
            var output = $"[{title}] {msg}";
            LambdaLogger.Log(output);
        }
    }
}