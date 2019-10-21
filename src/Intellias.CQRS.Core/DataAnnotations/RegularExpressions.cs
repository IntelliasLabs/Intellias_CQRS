namespace Intellias.CQRS.Core.DataAnnotations
{
    /// <summary>
    /// Common regular expressions that used across solution.
    /// </summary>
    public static class RegularExpressions
    {
        /// <summary>
        /// Regex for a string that contains only alphanumeric characters.
        /// </summary>
        public const string Alphanumeric = "^[a-zA-Z0-9_]*$";

        /// <summary>
        /// Regex for a string that contains a simplified list of ASCII characters.
        /// </summary>
        public const string SimplifiedAscii = @"^[a-zA-Z0-9_\-+=><~!?""'*#@&(){}\[\],.:;\/\\| ]*$";
    }
}