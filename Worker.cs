using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Learn.AsyncRequestReply
{
    public static class Worker
    {
        [FunctionName("Worker")]
        public static async Task Run([ServiceBusTrigger("thequeue", Connection = "ServiceBusConnectionString")]Message myQueueItem,
        [Blob("greetings", FileAccess.ReadWrite, Connection = "StorageConnectionString")] CloudBlobContainer blobContainer,
        ILogger log)
        {
            log.LogInformation("STARTING WORKER");
            log.LogInformation($"MESSAGE {System.Text.Encoding.UTF8.GetString(myQueueItem.Body)}");
            var body = JsonConvert.DeserializeObject<MessageBody>(System.Text.Encoding.UTF8.GetString(myQueueItem.Body));
            var blob = blobContainer.GetBlockBlobReference($"{body.Id}");
            log.LogInformation($"BODY ID {body.Id} name {body.Name}");
            /*string responseMessage = string.IsNullOrEmpty(body.Name)
                ? "The worker function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {body.Name}. This HTTP triggered function executed successfully.";
            await blob.UploadTextAsync(responseMessage);*/

            await blob.UploadFromByteArrayAsync(myQueueItem.Body, 0, myQueueItem.Body.Length);
        }
    }
}