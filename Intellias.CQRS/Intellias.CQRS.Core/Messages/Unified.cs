using System;

namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Unified unique codes generator designed by Sergey Seletsky
    /// </summary>
    public static class Unified
    {
        // FNV x64 Prime
        private const ulong Prime = 14695981039346656037U;

        // FNV x64 Offset
        private const ulong Offset = 1099511628211U;

        // Set of symbols used for numeric dimensions transcoding
        private static char[] symbols =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V'
        };

        // Generate x64 FNV hash based on random GUID
        public static ulong NewHash()
        {
            return NewHash(Guid.NewGuid().ToByteArray());
        }

        // Generate x64 FNV hash based on data bytes
        public static ulong NewHash(byte[] bytes)
        {
            var hash = Prime; // fnv prime

            foreach (var @byte in bytes)
            {
                hash ^= @byte;
                hash *= Offset; // fnv offset
            }

            return hash;
        }

        // Generate random x32 hex
        public static string NewCode()
        {
            return NewCode(NewHash());
        }

        // Generate x32 hex from number
        public static string NewCode(ulong n)
        {
            var len = 13;
            var ch = new char[len--];
            for (var i = len; i >= 0; i--)
            {
                var inx = (byte)((uint)(n >> (5 * i)) & 31);
                ch[len - i] = symbols[inx];
            }

            return new string(ch);
        }

        // Decode x32 hex to number
        public static ulong Decode(string code)
        {
            var shift = 5; // shift for x32 dimensions
            ulong hash = 0;
            for (int i = 0; i < code.Length; i++)
            {
                var index = (ulong)Array.IndexOf(symbols, code[i]);
                var nuim = index << ((code.Length - 1 - i) * shift); // convert dimension to number and add
                hash += nuim;
            }

            return hash;
        }
    }
}
