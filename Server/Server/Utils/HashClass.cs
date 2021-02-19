using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils
{
    public class HashClass
    {
        public static string GetHash(string input)
        {
            using var sha256 = SHA256.Create();
            var sourceBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha256.ComputeHash(sourceBytes);
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }
    }
}
