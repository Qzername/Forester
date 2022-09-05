using Avalonia.Media.Imaging;
using Forester.Code.Models.Pictures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Forester.Code.AppData
{
    public static class ImageData
    {
        static Dictionary<ElementSearch, ElementPicutres> ImageDatabase;

        static ImageData()
        {
            ImageDatabase = new Dictionary<ElementSearch, ElementPicutres>();
        }

        /// <summary>
        /// Wzięcie zdjęcia z bazy
        /// </summary>
        public static Bitmap GetImage(string name, ObjectType objectType, PictureType pictureType)
        {
            ElementSearch search = new ElementSearch()
            {
                Name = name,
                ObjectType = objectType,
                PictureType = pictureType
            };

            return GetImage(search);
        }

        /// <summary>
        /// Wzięcie zdjęcia z bazy
        /// </summary>
        public static Bitmap GetImage(ElementSearch search)
        {
            if (!ImageDatabase.ContainsKey(search))
            {
                var pp = ServerConnection.GetImage(search.Name, (int)search.ObjectType, 0);
                var bp = ServerConnection.GetImage(search.Name, (int)search.ObjectType, 1);

                ElementPicutres newElement = new ElementPicutres()
                {
                    IsDefaultProfilePicture = pp.Item2,
                    ProfilePicture = pp.Item1,
                    IsDefaultBackgroundPicture = bp.Item2,
                    BackgroundPicture = bp.Item1
                };

                ImageDatabase[search] = newElement;
            }

            if (search.PictureType == PictureType.ProfilePicture)
                return ImageDatabase[search].ProfilePicture;
            else
                return ImageDatabase[search].BackgroundPicture;
        }


        /// <summary>
        /// Sprawdzenie czy dana rzecz ma zdjęcie
        /// </summary>
        public static bool HasImage(string name, ObjectType objectType, PictureType pictureType)
        {
            ElementSearch search = new ElementSearch()
            {
                Name = name,
                ObjectType = objectType,
                PictureType = pictureType
            };

            bool returnValue;
            var obj = ImageDatabase[search];

            if (pictureType == PictureType.ProfilePicture)
                returnValue = obj.IsDefaultProfilePicture;
            else
                returnValue = obj.IsDefaultBackgroundPicture;

            return returnValue;
        }
    }
}
