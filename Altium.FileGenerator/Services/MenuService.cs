using Altium.FileGenerator.Models;

namespace Altium.FileGenerator.Services
{
    public static class MenuService
    {
        public static long GetFileSize()
        {
            var isCorrectInput = false;
            var count = 0L;
            while (!isCorrectInput)
            {
                Console.WriteLine("Enter size of the file in the format 100M - for 100 MB file, 10G - for 10 GB file");

                var sizeString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(sizeString))
                {
                    ErrorMessages.PrintFormatError();
                    continue;
                }

                var lastChar = sizeString[sizeString.Length - 1];
                if (lastChar != 'M' && lastChar != 'G')
                {
                    ErrorMessages.PrintFormatError();
                    continue;
                }

                var stringNumber = sizeString.Remove(sizeString.Length - 1, 1);
                var success = int.TryParse(stringNumber, out var number);
                if (!success)
                {
                    ErrorMessages.PrintFormatError();
                    continue;
                }

                count = lastChar switch
                {
                    'M' => 1024L * 1024L * number,
                    _ => 1024L * 1024L * 1024L * number,
                };

                isCorrectInput = true;
            }

            return count;
        }
    }
}
