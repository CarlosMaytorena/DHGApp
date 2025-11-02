using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
public class BlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobStorageService(string connectionString, string containerName)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerName = containerName;

        // Create the container if it doesn't exist
        //var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        //blobContainerClient.CreateIfNotExists();
    }
    public async Task UploadFileAsync(IFormFile file, string fileName)
    {
        try
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders { ContentType = file.ContentType });
            }
        }
        catch 
        {
            throw new Exception();
        }
    }
    public async Task<BlobDownloadInfo> DownloadFileAsync(string blobName)
    {
        string downloadFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),"Downloads",blobName); 
        try
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            Console.WriteLine($"Downloading blob '{blobName}' to '{downloadFilePath}'");
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            return download;
        }
        catch
        {
            throw new Exception();
        }
    }
}
