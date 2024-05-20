using Amazon.S3.Model;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Utilidades
{
    public class AmazonS3Helper
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _cloudFrontDomain;

        public AmazonS3Helper(string awsAccessKeyId, string awsSecretAccessKey, string region, string bucketName, string cloudFrontDomain)
        {
            _bucketName = bucketName;
            _cloudFrontDomain = cloudFrontDomain;
            _s3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, Amazon.RegionEndpoint.GetBySystemName(region));
        }

        public string GenerateCloudFrontPreSignedURL(string objectKey, double durationInMinutes = 10)
        {
            var s3Url = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                Expires = DateTime.UtcNow.AddMinutes(durationInMinutes)
            });

            var uri = new Uri(s3Url);
            var query = uri.Query;

            var cloudFrontUrl = $"https://{_cloudFrontDomain}/{objectKey}{query}";
            return cloudFrontUrl;
        }
    }

}
