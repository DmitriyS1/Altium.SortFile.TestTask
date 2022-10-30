namespace Altium.FileGenerator.Models
{
    /// <summary>
    /// Contains constants and methods to show Input errors
    /// </summary>
    public static class ErrorMessages
    {
        private static string WRONG_FORMAT = "Wrong format! Please, try again\n";

        /// <summary>
        /// Prints wrong format error
        /// </summary>
        public static void PrintFormatError()
        {
            Console.WriteLine(WRONG_FORMAT);
        }
    }
}
