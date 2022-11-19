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
            //var fileName = GetLastFileName();

            //var countOfUnsorted = await SplitFile(fileName);

            //for (int i = 0; i < countOfUnsorted; i++)
            //{
            //    var unsortedFilePath = $"{FILE_DIRECTORY}/{UNSORTED}-{i}.json";
            //    var sortedArray = await SortFile(unsortedFilePath);
            //    await WriteAndSerializeArray(sortedArray, i, true);
            //    File.Delete(unsortedFilePath);
            //}

            var filesToMerge = GetSortedFileNames();
            var resultName = "";
            while(filesToMerge.Count > 1)
            {
                var firstName = $"{FILE_DIRECTORY}/{filesToMerge.Dequeue()}";
                var secondName = $"{FILE_DIRECTORY}/{filesToMerge.Dequeue()}";

                resultName = await MergeSortedFiles(firstName, secondName);

                File.Delete(firstName);
                File.Delete(secondName);

                filesToMerge.Enqueue($"{FILE_DIRECTORY}/{resultName}");
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

        private Queue<string> GetSortedFileNames()
        {
            var directory = new DirectoryInfo(FILE_DIRECTORY);

            // TODO: Create a menu where you can choose from all txt files in directory
            var myFile = directory.GetFiles()
                .Where(x => x.Name.Contains("sorted"))
                .Select(file => file.Name);

            return new Queue<string>(myFile);
        }

        private async Task<string> MergeSortedFiles(string path1, string path2, string extension = "json")
        {
            var name1 = path1.Split('/').Last().Split('.')[0].Split('-').Last(); // sorted-0
            var name2 = path2.Split('/').Last().Split('.')[0].Split('-').Last(); // sorted-1

            using var firstFileStream = File.OpenText(path1);
            using var secondFileStream = File.OpenText(path2);

            var resultFileName = $"{FILE_DIRECTORY}/merged-{name1}{name2}.{extension}";
            using var resultFile = new StreamWriter(resultFileName);
            var currentLine1 = JsonSerializer.Deserialize<Line>(await firstFileStream.ReadLineAsync());
            var currentLine2 = JsonSerializer.Deserialize<Line>(await secondFileStream.ReadLineAsync());
            while(!secondFileStream.EndOfStream && !firstFileStream.EndOfStream)
            {
                var compareRes = currentLine1.CompareTo(currentLine2);
                if (compareRes < 0)
                {
                    await resultFile.WriteLineAsync(JsonSerializer.Serialize(currentLine1));
                    if (!firstFileStream.EndOfStream)
                    {
                        currentLine1 = JsonSerializer.Deserialize<Line>(await firstFileStream.ReadLineAsync());
                    }
                }
                else
                {
                    await resultFile.WriteLineAsync(JsonSerializer.Serialize(currentLine2));
                    if (!secondFileStream.EndOfStream)
                    {
                        currentLine2 = JsonSerializer.Deserialize<Line>(await secondFileStream.ReadLineAsync());
                    }
                }
            }

            return resultFileName; 
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
