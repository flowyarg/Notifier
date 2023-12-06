using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Notifier.Logic.Services
{
    public class AuthenticatorService
    {
        private static readonly Dictionary<string, char> _binaryToHexTable = new()
        {
            ["0000"] = '0',
            ["0001"] = '1',
            ["0010"] = '2',
            ["0011"] = '3',
            ["0100"] = '4',
            ["0101"] = '5',
            ["0110"] = '6',
            ["0111"] = '7',
            ["1000"] = '8',
            ["1001"] = '9',
            ["1010"] = 'A',
            ["1011"] = 'B',
            ["1100"] = 'C',
            ["1101"] = 'D',
            ["1110"] = 'E',
            ["1111"] = 'F',
        };

        private const string _base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private const string _hexChars = "0123456789ABCDEF";
        private const int _stepDurationInSeconds = 30;
        private readonly DateTimeOffset _beginningOfTime = new (1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        private readonly byte[] _hexKeyBytes;

        public AuthenticatorService(string base32key)
        {
            _hexKeyBytes = HexStr2Bytes(Base32Str2HexStr(base32key));
        }

        public string GetAuthenticationCode(DateTimeOffset dateTime)
        {
            var epoch = Math.Round((dateTime - _beginningOfTime).TotalSeconds, 0);
            var timeStepsCount = (long)Math.Floor(epoch / _stepDurationInSeconds);

            var steps = DecimalToHex(timeStepsCount);

            var alignment = new string('0', 16 - steps.Length);
            steps = alignment + steps;

            // Get the HEX in a Byte[]
            var message = HexStr2Bytes(steps);

            var provider = new HMACSHA1(_hexKeyBytes);
            provider.Initialize();
            var hash = provider.ComputeHash(message);

            var hmacHex = BytesToHexString(hash);

            var offset = HexToDecimal(hmacHex[(hmacHex.Length - 1)..]);
            //var part1 = hmacHex[..(offset * 2)];
            var part2 = hmacHex.Substring(offset * 2, 8);
            //var part3 = hmacHex[(offset * 2 + 8)..];

            var code = (HexToDecimal(part2) & 0x7FFFFFFF).ToString();
            code = code[(code.Length - 6)..];
            return code;
        }

        private static string Base32Str2HexStr(string base32)
        {
            base32 = base32.ToUpper();
            StringBuilder bits = new();

            for (var i = 0; i < base32.Length; i++)
            {
                var decimalValue = _base32Chars.IndexOf(base32[i]);
                var binaryValue = Convert.ToString(decimalValue, 2);
                bits.Append('0', 5 - binaryValue.Length);
                bits.Append(binaryValue);
            }

            if (bits.Length % 4 != 0)
            {
                bits.Insert(0, "0", 4 - bits.Length % 4);
            }

            var hex = new string(bits.ToString().Chunk(4).Select(chunk => _binaryToHexTable[new string(chunk)]).ToArray());

            return hex;
        }

        private static byte[] HexStr2Bytes(string hex)
        {
            // Adding one byte to get the right conversion
            // Values starting with "0" can be converted
            var bArray = BigInteger.Parse("10" + hex, NumberStyles.HexNumber).ToByteArray();

            // Copy all the REAL bytes, not the "first"
            // Copying in reverse order, because bytes in BigInteger are reversed
            var result = new byte[bArray.Length - 1];
            for (var i = 0; i < result.Length; i++)
                result[i] = bArray[bArray.Length - 2 - i];
            return result;
        }

        private static string BytesToHexString(byte[] bytes)
        {
            StringBuilder result = new();
            for (var i = 0; i < bytes.Length; i++)
            {
                result.Append(_hexChars[bytes[i] / 16]);
                result.Append(_hexChars[bytes[i] % 16]);
            }
            return result.ToString();
        }

        private static int HexToDecimal(string hex)
        {
            return int.Parse(hex, NumberStyles.HexNumber);
        }

        private static string DecimalToHex(long number)
        {
            return string.Format("{0:X}", number).ToUpper();
        }
    }
}
