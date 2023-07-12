using Avalonia.Media.Imaging;
using Forester.Data;
using Forester.Models.API.Pictures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Drawing = System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform;
using Forester.Tools;

namespace Forester.Services
{
    public class PictureService
    {
        //dependency injection
        PictureDatabase pictureDatabase;
        ThemeService themeService;

        Dictionary<ImageIdentifier, Bitmap> Cache;

        public PictureService(PictureDatabase pictureDatabase, ThemeService themeService) 
        {
            this.pictureDatabase = pictureDatabase;
            this.themeService = themeService;

            Cache = new Dictionary<ImageIdentifier, Bitmap>();
        }

        public async Task<Bitmap> GetImage(string name, ObjectType objectType, PictureType pictureType)
        {
            return await GetImage(new ImageIdentifier()
            {
                Name = name,
                ObjectType = objectType,
                PictureType = pictureType
            });
        }

        async Task<Bitmap> GetImage(ImageIdentifier imageIdentifier)
        {
            if(!Cache.ContainsKey(imageIdentifier))
            {
                var picture = await pictureDatabase.GetImage(imageIdentifier.Name, imageIdentifier.ObjectType, imageIdentifier.PictureType);

                if (picture is null)
                {
                    if (imageIdentifier.PictureType == PictureType.ProfilePicture)
                        picture = new Bitmap(AssetLoader.Open(new Uri("avares://Forester/Assets/defaultPP.png")));
                    else
                        picture = GenerateGradient();
                }
               
                Cache.Add(imageIdentifier, picture);
            }

            return Cache[imageIdentifier];
        }

        public async Task UpdateImage(Bitmap bitmap, string name, ObjectType objectType, PictureType pictureType)
        {
            await UpdateImage(bitmap, new ImageIdentifier()
            {
                Name = name,
                ObjectType = objectType,
                PictureType = pictureType
            });
        }

        async Task UpdateImage(Bitmap bitmap, ImageIdentifier imageIdentifier)
        {
            await pictureDatabase.UploadImage(bitmap, imageIdentifier.Name, imageIdentifier.ObjectType, imageIdentifier.PictureType);

            Cache[imageIdentifier] = bitmap;
        }

        Bitmap GenerateGradient()
        { //does it works? work it does. Change it later tho bcs it can work better ig
            int height = 400, width = 1000;

            var brushColor = themeService.ThirdBrush.Color;

            var color = Drawing.Color.FromArgb(brushColor.R, brushColor.G, brushColor.B);

            using (Drawing.Bitmap bitmap = new Drawing.Bitmap(width, height))
            using (Drawing.Graphics graphics = Drawing.Graphics.FromImage(bitmap))
            using (LinearGradientBrush brush = new LinearGradientBrush(new Drawing.Point(0, 0), new Drawing.Point(height, width), Drawing.Color.Black, color))
            {
                brush.SetSigmaBellShape(0.8f);
                graphics.FillRectangle(brush, new Drawing.Rectangle(0, 0, width, height));

                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    return new Bitmap(memory);
                }
            }
        }

        struct ImageIdentifier
        {
            public string Name;
            public ObjectType ObjectType;
            public PictureType PictureType;
        }
    }
}
