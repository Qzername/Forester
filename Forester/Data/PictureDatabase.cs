using Avalonia.Media;
using Avalonia.Media.Imaging;
using Forester.Data.Connection;
using Forester.Models.API.Pictures;
using Forester.Services;
using Forester.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Data
{
    public class PictureDatabase : Database
    {
        protected override string APIprefix => "Pictures/";

        public PictureDatabase(FileTransferManager fileTransferManager, ErrorMessageService errorMessageService) :base(fileTransferManager, errorMessageService)
        {

        }

        public async Task<Bitmap?> GetImage(string name, ObjectType objectType, PictureType pictureType)
        {
            var bytes = await FileTransferManager.Download(GenerateURI($"?name={name}&objectType={objectType}&pictureType={pictureType}"));

            if (bytes.Length == 0)
                return null;

            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(bytes, 0, bytes.Length);
            memoryStream.Position = 0;

            Bitmap bitmap = new Bitmap(memoryStream);

            return bitmap;
        }

        public async Task UploadImage(Bitmap image, string name, ObjectType objectType, PictureType pictureType)
        {
            var stream = new MemoryStream();
            image.Save(stream);
            stream.Position = 0;

            await FileTransferManager.Upload(GenerateURI($"?name={name}&objectType={objectType}&pictureType={pictureType}"), stream, null);
        }
    }
}