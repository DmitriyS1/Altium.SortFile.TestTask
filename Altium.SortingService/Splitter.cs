using Altium.SortingService.Models;
using System.Text;
using System.Text.Json;

namespace Altium.SortingService
{
    public class Splitter
    {
        private readonly string FILE_DIRECTORY = "../../../../Altium.FileGenerator/bin/Debug/net6.0";
        private readonly string SORTED = "sorted";
        private readonly string UNSORTED = "unsorted";

        public async Task WorkingFileName()
        {
            var fileName = GetLastFileName();

            var countOfUnsorted = await SplitFile(fileName);

            for (int i = 0; i < countOfUnsorted; i++)
            {
                var unsortedFilePath = $"{FILE_DIRECTORY}/{UNSORTED}-{i}.json";
                var sortedArray = await SortFile(unsortedFilePath);
                await WriteAndSerializeArray(sortedArray, i, true);
                File.Delete(unsortedFilePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Count of files in the directory</returns>
        public async Task<int> SplitFile(string fileName)
        {
            // ToDo: Check for previous unsorted files
            using var stream = File.OpenText(FILE_DIRECTORY + "/" + fileName);
            stream.BaseStream.Position = 0;

            var counter = 0;
            var currentFileNumber = 0;
            var linesBatch = new List<Line>(10000);
            while (!stream.EndOfStream)
            {
                if (counter == 10000)
                {
                    await WriteAndSerializeArray(linesBatch, currentFileNumber++);
                    counter = 0;
                    linesBatch.Clear();
                }

                linesBatch.Add(ParseData(stream.ReadLine()));
                counter++;
            }

            await WriteAndSerializeArray(linesBatch, currentFileNumber++);

            return currentFileNumber;
        }

        private async Task MergeSortedFiles(string path1, string path2)
        {
            var name1 = path1.Split('/').Last().Split('.')[0];
            var name2 = path2.Split('/').Last().Split('.')[0];

            using var firstFileStream = File.OpenText(path1);
            using var secondFileStream = File.OpenText(path2);

            using var resultFile = File.OpenWrite($"{FILE_DIRECTORY}/{name1}-{name2}.txt");
            var currentLine1 = JsonSerializer.Deserialize<Line>(firstFileStream.ReadLine());
            var currentLine2 = JsonSerializer.Deserialize<Line>(second FileStream.ReadLine());
            while(!secondFileStream.EndOfStream && !firstFileStream.EndOfStream)
            {
                if
            }
        }

        private Line ParseData(string line)
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

        private async Task<Line[]> SortFile(string path)
        {
            var fileContent = await File.ReadAllTextAsync(path);
            var deserialized = JsonSerializer.Deserialize<Line[]>(fileContent);
            Array.Sort(deserialized);

            return deserialized;
        }

        private async Task WriteAndSerializeArray(IEnumerable<Line> lines, int fileNumber, bool isSorted = false)
        {
            var sortedString = isSorted ? SORTED : UNSORTED;
            await File.WriteAllTextAsync($"{FILE_DIRECTORY}/{sortedString}-{fileNumber}.json", JsonSerializer.Serialize(lines));
        }

        private string GetLastFileName()
        {
            var directory = new DirectoryInfo(FILE_DIRECTORY); 
            
            // TODO: Create a menu where you can choose from all txt files in directory
            var myFile = directory.GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault(x => x.Name.Contains("file-"));

            return myFile.Name;
        }
    }
}
