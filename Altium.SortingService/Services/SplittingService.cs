using Altium.SortingService.Models;
using Altium.SortingService.Utils;
using System.Text;

namespace Altium.SortingService.Services
{
    public class SplittingService
    {
        private readonly string _directory;
        private string UNSORTED = "unsorted";
        private string SORTED = "sorted";
        private readonly int _counter;

        /// <summary>
        /// Creates instance of the SplittingService class
        /// </summary>
        /// <param name="directory">Directory where to find the file</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="counter">Count of lines in the files. Depends on available RAM</param>
        /// <exception cref="Exception">Throws if file not found</exception>
        public SplittingService(
            string directory, 
            string fileName,
            int counter)
        {
            var isPathValid = File.Exists($"{directory}/{fileName}");
            if (!isPathValid)
            {
                Console.WriteLine("File not found");
                throw new Exception("FileNotFoundException");
            }

            _directory = directory;
            _counter = counter;
        }

        /// <summary>
        /// Splits file to several chunks
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Count of files</returns>
        public async Task<int> SplitFile(string fileName)
        {
            using var stream = File.OpenText($"{_directory}/{fileName}");
            stream.BaseStream.Position = 0;

            var counter = 0;
            var currentFileNumber = 0;
            var linesBatch = new List<Line>(_counter);
            while (!stream.EndOfStream)
            {
                if (counter == _counter)
                {
                    linesBatch.Sort();
                    await linesBatch.WriteAndSerialize(currentFileNumber++, SORTED, _directory);
                    counter = 0;
                    linesBatch.Clear();
                }

                linesBatch.Add(ParseLine(await stream.ReadLineAsync()));
                counter++;
            }

            linesBatch.Sort();
            await linesBatch.WriteAndSerialize(currentFileNumber++, SORTED, _directory);

            return currentFileNumber;
        }

        private Line ParseLine(string line)
        {
            var lineParts = line.Split('.');
            var result = new Line
            {
                SerialNumber = int.Parse(lineParts[0])
            };

            var nameBuilder = new StringBuilder();
            for (int i = 1; i < lineParts.Length; i++)
            {
                nameBuilder.Append(lineParts[i]);
            }

            result.CompanyName = nameBuilder.ToString();
            return result;
        }
    }
}
