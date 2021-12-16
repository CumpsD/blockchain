namespace Blockchain.Infrastructure
{
    using System;
    using System.Globalization;
    using System.Text;

    public static class ConvertFromHexExtension
    {
        public static string ConvertFromHex(this string hexString, Encoding encoding)
            => ConvertFromHex(hexString.AsSpan(), encoding);

        public static string ConvertFromHex(this ReadOnlySpan<char> hexString, Encoding encoding)
        {
            var realLength = 0;
            for (var i = hexString.Length - 2; i >= 0; i -= 2)
            {
                var b = byte.Parse(hexString.Slice(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                if (b == 0)
                    continue;

                realLength = i + 2;
                break;
            }

            var bytes = new byte[realLength / 2];
            for (var i = 0; i < bytes.Length; i++)
                bytes[i] = byte.Parse(hexString.Slice(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

            return encoding.GetString(bytes);
        }
    }
}
