using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace AsyncRequestReply
{
    public static class Gate
    {
        [FunctionName("Gate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [ServiceBus("thequeue", Connection = "ServiceBusConnectionString")] IAsyncCollector<Message> OutMessage,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name ??= data?.name;

            var messageBody = new MessageBody{
                Id = Guid.NewGuid().ToString(),
                Name = name
            };
            string bodyAsJson = JsonConvert.SerializeObject(messageBody);

            await OutMessage.AddAsync(new Message(Encoding.UTF8.GetBytes(bodyAsJson)));

            var statusFunctionKey = Environment.GetEnvironmentVariable("StatusFunctionKey");
            var statusFunctionHost = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
            string statusUrl = $"https://{statusFunctionHost}/api/Status/{messageBody.Id}?code={statusFunctionKey}";
            return new AcceptedResult(statusUrl, "Request accepted");
        }
    }
}
