using System.Security.Cryptography;
using System.Text;

namespace Notifier.Logic.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Hashes a string using <see cref="SHA512"/> algorithm
        /// </summary>
        /// <param name="inputString">A string to hash</param>
        /// <returns>A hash result as byte array</returns>
        public static byte[] GetStringHash512(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                throw new ArgumentNullException(nameof(inputString));
            return SHA512.HashData(Encoding.Unicode.GetBytes(inputString));
        }
    }
}
