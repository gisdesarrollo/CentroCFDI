using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace AWS
{
    public class AmazonS3Uploader
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public AmazonS3Uploader(string awsAccessKeyId, string awsSecretAccessKey, string region, string bucketName)
        {
            _bucketName = bucketName;
            _s3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, Amazon.RegionEndpoint.GetBySystemName(region));
        }

        public async Task UploadFileAsync(Stream inputStream, string key, string contentType)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(_s3Client);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = inputStream,
                    Key = key,
                    BucketName = _bucketName,
                    ContentType = contentType,
                };

                await fileTransferUtility.UploadAsync(uploadRequest);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                throw new Exception("Error occurred: " + amazonS3Exception.Message);
            }
        }
    }
}
