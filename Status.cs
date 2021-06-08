using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace Learn.AsyncRequestReply
{
    public static class Status
    {
        [FunctionName("Status")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Blob("greetings", FileAccess.ReadWrite, Connection = "StorageConnectionString")] BlobContainerClient blobContainer,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string id = req.Query["id"];
            var blobClient = blobContainer.GetBlobClient(id);
            if (!await blobClient.ExistsAsync()) return new AcceptedResult();
            var tomorrow = DateTime.UtcNow.AddDays(1);
            var sasUri = blobClient.GenerateSasUri(new Azure.Storage.Sas.BlobSasBuilder(Azure.Storage.Sas.BlobSasPermissions.Read, new DateTimeOffset(tomorrow)));
            return new RedirectResult(sasUri.AbsoluteUri);
        }
    }
}
