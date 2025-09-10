using Amazon.S3.Transfer;
using Amazon.S3;
using NTools.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Domain;

namespace NTools.Domain.Impl.Services
{
    public class ImageService : IImageService
    {

        private const string ACCESS_KEY = "DO00JY46P2RAD368YY3B";
        private const string SECRET_KEY = "6aojG9/UVwcn9Ss8mT7HNCPPUCk2GF1bG/CarPcC5n0";
        private const string BUCKET_NAME = "nauth";
        //private const string REGION = "nyc3"; // ou fra1, sgp1, etc.
        private const string ENDPOINT = "https://emagine.nyc3.digitaloceanspaces.com";

        public string GetImageUrl(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                return ENDPOINT + "/" + BUCKET_NAME + "/" + fileName;
            }
            return string.Empty;
        }

        private void UploadFile(Stream fileStream, string fileName)
        {
            var config = new AmazonS3Config
            {
                ServiceURL = ENDPOINT,
                ForcePathStyle = true,
                //SignatureVersion = "v4",
            };

            using var client = new AmazonS3Client(ACCESS_KEY, SECRET_KEY, config);
            var transferUtility = new TransferUtility(client);

            var request = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = fileName,
                BucketName = BUCKET_NAME,
                CannedACL = S3CannedACL.PublicRead // ou Private se quiser restrito
            };

            transferUtility.Upload(request);
        }

        public string InsertFromStream(Stream stream, string name)
        {
            UploadFile(stream, name);
            return name;
        }

        /*
        public string InsertToUser(Stream stream, long userId)
        {
            if (!(userId > 0))
            {
                throw new Exception("Invalid User ID");
            }
            var user = _userFactory.BuildUserModel().GetById(userId, _userFactory);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var name = string.Format("user-{0}.jpg", StringUtils.GenerateShortUniqueString());
            UploadFile(stream, name);
            user.Image = name;
            user.Update(_userFactory);
            return name;
        }
        */
    }
}
