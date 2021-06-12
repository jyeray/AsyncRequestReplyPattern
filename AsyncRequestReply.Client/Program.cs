using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AsyncRequestReply.Client
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
            Console.WriteLine("Please enter a name:");
            var name = Console.ReadLine();
            var statusUrl = await CallGateFunction(configuration["gateUrl"], name);
            var statusHttpReponseMessage = await CallStatusFunctionUntilResourceCreated(statusUrl);
            var response = await statusHttpReponseMessage.Content.ReadAsStringAsync();
            Console.WriteLine();
            Console.WriteLine($"Resource created, the resulting message is: '{response}'");
        }

        public static async Task<string> CallGateFunction(string gateUrl, string name) {
            Console.WriteLine();
            Console.WriteLine($"Making api call to Gate function: {gateUrl}");
            var gateHttpResponseMessage = await client.PostAsJsonAsync(gateUrl, new RequestBody{Name = name});
            var gateResponse = await gateHttpResponseMessage.Content.ReadAsStringAsync();
            Console.WriteLine($"Gate function responded with http code: '{gateHttpResponseMessage.StatusCode}' and message: '{gateResponse}'");
            return gateHttpResponseMessage.Headers.GetValues("Location").First();
        }

        public static async Task<HttpResponseMessage> CallStatusFunctionUntilResourceCreated(string statusFunctionUrl) {
            HttpResponseMessage httpReponseMessage;
            do {
                Console.WriteLine();
                Console.WriteLine($"Making api call to Status function: {statusFunctionUrl}");
                httpReponseMessage = await client.GetAsync(statusFunctionUrl);
                Console.WriteLine($"Status function responded with http code: '{httpReponseMessage.StatusCode}'");
                Thread.Sleep(100);
            } while(httpReponseMessage.StatusCode == HttpStatusCode.Accepted);
            return httpReponseMessage;
        }
    }
}
