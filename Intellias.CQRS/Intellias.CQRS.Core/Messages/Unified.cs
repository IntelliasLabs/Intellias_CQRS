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

        /// <summary>
        /// Generate x64 FNV hash based on random GUID
        /// </summary>
        /// <returns>Guid based FNV hash</returns>
        public static ulong NewHash()
        {
            return NewHash(Guid.NewGuid().ToByteArray());
        }

        /// <summary>
        /// Generate x64 FNV hash based on data bytes
        /// </summary>
        /// <param name="bytes">Source data</param>
        /// <returns>FNV hash</returns>
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

        /// <summary>
        /// Generate random x32 hex
        /// </summary>
        /// <returns>x32 hex based on guid</returns>
        public static string NewCode()
        {
            return NewCode(NewHash());
        }

        /// <summary>
        /// Generate x32 hex from number
        /// </summary>
        /// <param name="hash">hash</param>
        /// <returns></returns>
        public static string NewCode(ulong hash)
        {
            var len = 13;
            var ch = new char[len--];
            for (var i = len; i >= 0; i--)
            {
                var inx = (byte)((uint)(hash >> (5 * i)) & 31);
                ch[len - i] = symbols[inx];
            }

            return new string(ch);
        }

        /// <summary>
        /// Decode x32 hex to number
        /// </summary>
        /// <param name="code">Unified code</param>
        /// <returns>FNV hash</returns>
        public static ulong Decode(string code)
        {
            var shift = 5; // shift for x32 dimensions
            ulong hash = 0;
            for (var i = 0; i < code.Length; i++)
            {
                var index = (ulong)Array.IndexOf(symbols, code[i]);
                var nuim = index << ((code.Length - 1 - i) * shift); // convert dimension to number and add
                hash += nuim;
            }

            return hash;
        }
    }
}
