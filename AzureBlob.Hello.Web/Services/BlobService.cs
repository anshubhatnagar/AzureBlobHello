using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlob.Hello.Web.Services
{
    public class BlobService
    {
        public async Task UploadBlob(string fileName, Stream stream, string connectionString)
        {
            CloudBlobClient cloudBlobClient = GetBlobClient(connectionString);
            CloudBlobContainer cloudBlobContainer = await GetBlobContainer(cloudBlobClient);

            // Get a reference to the blob address, then upload the file to the blob.
            // Use the value of localFileName for the blob name.
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            await cloudBlockBlob.UploadFromStreamAsync(stream);
        }

        public async Task<List<string>> GetBlobList(string connectionString)
        {
            CloudBlobClient cloudBlobClient = GetBlobClient(connectionString);
            CloudBlobContainer cloudBlobContainer = await GetBlobContainer(cloudBlobClient);
            BlobContinuationToken blobContinuationToken = null;
            List<string> imageList = new List<string>();

            do
            {
                var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                foreach (IListBlobItem item in results.Results)
                {
                    imageList.Add(item.Uri.ToString());
                }
            } while (blobContinuationToken != null); // Loop while the continuation token is not null.

            return imageList;
        }

        private CloudBlobClient GetBlobClient(string connectionString)
        {
            CloudStorageAccount storageAccount;

            if (!CloudStorageAccount.TryParse(connectionString, out storageAccount))
            {
                throw new InvalidOperationException("Unable to connect Azure Storage Account.");
            }

            // Create the CloudBlobClient that represents the 
            // Blob storage endpoint for the storage account.
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

            return cloudBlobClient;
        }

        private async Task<CloudBlobContainer> GetBlobContainer(CloudBlobClient cloudBlobClient)
        {
            // Create a container called 'quickstartblobs' and 
            // append a GUID value to it to make the name unique.
            CloudBlobContainer cloudBlobContainer =
                cloudBlobClient.GetContainerReference("imagesblob");

            if (!await cloudBlobContainer.ExistsAsync())
            {
                await cloudBlobContainer.CreateAsync();

                // Set the permissions so the blobs are public.
                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                await cloudBlobContainer.SetPermissionsAsync(permissions);
            }

            return cloudBlobContainer;
        }
    }
}
