using System.Security.Cryptography;
using System.Text;

namespace CineBenavides.Helpers
{
    public class HashHelpers
    {
        public static string ObtenerHash(string texto)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
