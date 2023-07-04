using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
using System.Text;

namespace ForesterAPI.Tools
{
    public static class EncryptionManager
    {
        public static string Encrypt(string password)
        {
            byte[] data = Encoding.UTF8.GetBytes(password);
            byte[] result;
            SHA256 shaM = new SHA256Managed();
            result = shaM.ComputeHash(data);

            return Convert.ToHexString(result);
        }
    }
}
