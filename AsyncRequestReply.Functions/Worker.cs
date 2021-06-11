using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AsyncRequestReply
{
    public static class Worker
    {
        [FunctionName("Worker")]
        public static async Task Run([ServiceBusTrigger("thequeue", Connection = "ServiceBusConnectionString")]Message myQueueItem,
        [Blob("greetings", FileAccess.ReadWrite, Connection = "StorageConnectionString")] BlobContainerClient blobContainer,
        ILogger log)
        {
            log.LogInformation("STARTING WORKER");
            log.LogInformation($"MESSAGE {System.Text.Encoding.UTF8.GetString(myQueueItem.Body)}");
            Thread.Sleep(500);
            var body = JsonConvert.DeserializeObject<MessageBody>(System.Text.Encoding.UTF8.GetString(myQueueItem.Body));
            log.LogInformation($"BODY ID {body.Id} name {body.Name}");

            string responseMessage = string.IsNullOrEmpty(body.Name)
                ? "The worker function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {body.Name}. The Async Request Reply pattern worked as expected.";

            var blobClient = blobContainer.GetBlobClient(body.Id);
            await blobClient.UploadAsync(new BinaryData(Encoding.UTF8.GetBytes(responseMessage)));
        }
    }
}