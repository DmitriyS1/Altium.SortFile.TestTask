using Altium.SortingService.Models;
using Altium.SortingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Altium.SortingService.Services
{
    public class SplittingService
    {
        private readonly string FILE_DIRECTORY; // = "../../../../Altium.FileGenerator/bin/Debug/net6.0";
        private string UNSORTED = "unsorted";

        public SplittingService(string directory)
        {
            FILE_DIRECTORY = directory;
        }

        /// <summary>
        /// Splits file to several chunks
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Count of files</returns>
        public async Task<int> SplitFile(string fileName)
        {
            // ToDo: Check for previous unsorted files
            using var stream = File.OpenText($"{FILE_DIRECTORY}/{fileName}");
            stream.BaseStream.Position = 0;

            var counter = 0;
            var currentFileNumber = 0;
            var linesBatch = new List<Line>(10000);
            while (!stream.EndOfStream)
            {
                if (counter == 10000)
                {
                    await linesBatch.WriteAndSerialize(currentFileNumber++, UNSORTED, FILE_DIRECTORY);
                    counter = 0;
                    linesBatch.Clear();
                }

                linesBatch.Add(ParseLine(stream.ReadLine()));
                counter++;
            }

            await linesBatch.WriteAndSerialize(currentFileNumber++, UNSORTED, FILE_DIRECTORY);

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
