using Amazon.S3.Model;
using Amazon.S3;
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;

namespace Aplicacion.Utilidades
{
    public class AmazonS3Downloader
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public AmazonS3Downloader(string awsAccessKeyId, string awsSecretAccessKey, string region, string bucketName)
        {
            _bucketName = bucketName;
            _s3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.GetBySystemName(region));
        }

        public async Task<Stream> DownloadFileAsync(string key)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                using (var response = await _s3Client.GetObjectAsync(request))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    await response.ResponseStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0; // Reset the stream position to the beginning
                    return memoryStream;
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                throw new Exception("Error occurred: " + amazonS3Exception.Message);
            }
        }
    }
}
