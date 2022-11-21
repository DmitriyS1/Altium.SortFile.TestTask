using Altium.SortingService.Models;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Altium.SortingService.Services
{
    /// <summary>
    /// Class to merge files into one
    /// </summary>
    public class MergingService
    {
        private readonly string _directory;

        public MergingService(string directory)
        {
            _directory = directory;
        }

        /// <summary>
        /// Merges bunch of files into one
        /// </summary>
        /// <param name="filesToMerge">File names</param>
        /// <param name="fileName">Name of the file to sort</param>
        /// <returns>Name of the sorted file</returns>
        public async Task<string> Merge(Queue<string> filesToMerge, string fileName)
        {
            var resultName = "";
            while (filesToMerge.Count > 2)
            {
                var firstPath = $"{_directory}{filesToMerge.Dequeue()}";
                var secondPath = $"{_directory}{filesToMerge.Dequeue()}";

                resultName = await MergeSortedFiles(firstPath, secondPath, $"merged-{Guid.NewGuid()}.txt");

                File.Delete(firstPath);
                File.Delete(secondPath);

                filesToMerge.Enqueue(resultName);
            }

            string firstPath1 = $"{_directory}{filesToMerge.Dequeue()}";
            var secondPath1 = $"{_directory}{filesToMerge.Dequeue()}";

            resultName = await MergeSortedFiles(firstPath1, secondPath1, $"{fileName}-sorted.txt", false);

            File.Delete(firstPath1);
            File.Delete(secondPath1);

            return resultName;
        }

        /// <summary>
        /// Merges bunch of files into one
        /// </summary>
        /// <param name="filesToMerge">File names</param>
        /// <param name="fileName">Name of the file to sort</param>
        /// <returns>Name of the sorted file</returns>
        public string MergeInParallel(ConcurrentQueue<string> filesToMerge, string fileName)
        {
            while(filesToMerge.Count > 2)
            {
                Parallel.For(0, filesToMerge.Count / 2, (i) =>
                {
                    var isFirstSuccess = filesToMerge.TryDequeue(out var firstName);
                    var isSecondSuccess = filesToMerge.TryDequeue(out var secondName);

                    var firstPath = $"{_directory}{firstName}";
                    var secondPath = $"{_directory}{secondName}";

                    var resultName = MergeSortedFilesParallel(firstPath, secondPath);

                    File.Delete(firstPath);
                    File.Delete(secondPath);

                    filesToMerge.Enqueue(resultName);
                });

            }

            var isFirstSuccess = filesToMerge.TryDequeue(out var firstName);
            var isSecondSuccess = filesToMerge.TryDequeue(out var secondName);

            var finalPath1 = $"{_directory}{firstName}";
            var finalPath2 = $"{_directory}{secondName}";

            var resultName = FinalMergeParallel(finalPath1, finalPath2, fileName);
            
            File.Delete(finalPath1);
            File.Delete(finalPath2);

            return resultName;
        }

        private async Task<string> MergeSortedFiles(string firstPath, string secondPath, string resultFileName, bool isJson = true)
        {
            using var firstFileStream = File.OpenText(firstPath);
            using var secondFileStream = File.OpenText(secondPath);

            using var resultFile = new StreamWriter($"{_directory}{resultFileName}");

            var currentLine1 = JsonSerializer.Deserialize<Line>(await firstFileStream.ReadLineAsync());
            var currentLine2 = JsonSerializer.Deserialize<Line>(await secondFileStream.ReadLineAsync());

            while (!secondFileStream.EndOfStream && !firstFileStream.EndOfStream)
            {
                var compareRes = currentLine1.CompareTo(currentLine2);
                if (compareRes < 0)
                {
                    await WriteData(resultFile, currentLine1, isJson);
                    if (!firstFileStream.EndOfStream)
                    {
                        currentLine1 = JsonSerializer.Deserialize<Line>(await firstFileStream.ReadLineAsync());
                    }
                }
                else
                {
                    await WriteData(resultFile, currentLine2, isJson);
                    if (!secondFileStream.EndOfStream)
                    {
                        currentLine2 = JsonSerializer.Deserialize<Line>(await secondFileStream.ReadLineAsync());
                    }
                }
            }

            return resultFileName;
        }

        private async Task WriteData(StreamWriter writer, Line data, bool isJson)
        {
            if (isJson)
            {
                await writer.WriteLineAsync(JsonSerializer.Serialize(data));
            }
            else
            {
                await writer.WriteLineAsync($"{data.SerialNumber}. {data.CompanyName}");
            }
        }

        private string MergeSortedFilesParallel(string path1, string path2)
        {
            var number1 = path1.Split('/').Last().Split('.')[0].Split('-').Last(); // sorted-0
            var number2 = path2.Split('/').Last().Split('.')[0].Split('-').Last(); // sorted-1

            using var firstFileStream = File.OpenText(path1);
            using var secondFileStream = File.OpenText(path2);

            var resultFileName = $"merged-{Guid.NewGuid()}.txt";
            using var resultFile = new StreamWriter($"{_directory}{resultFileName}");

            var currentLine1 = JsonSerializer.Deserialize<Line>(firstFileStream.ReadLine());
            var currentLine2 = JsonSerializer.Deserialize<Line>(secondFileStream.ReadLine());

            while (!secondFileStream.EndOfStream && !firstFileStream.EndOfStream)
            {
                var compareRes = currentLine1.CompareTo(currentLine2);
                if (compareRes < 0)
                {
                    resultFile.WriteLine(JsonSerializer.Serialize(currentLine1));
                    if (!firstFileStream.EndOfStream)
                    {
                        currentLine1 = JsonSerializer.Deserialize<Line>(firstFileStream.ReadLine());
                    }
                }
                else
                {
                    resultFile.WriteLine(JsonSerializer.Serialize(currentLine2));
                    if (!secondFileStream.EndOfStream)
                    {
                        currentLine2 = JsonSerializer.Deserialize<Line>(secondFileStream.ReadLine());
                    }
                }
            }

            return resultFileName;
        }

        private string FinalMergeParallel(string path1, string path2, string finalName)
        {
            using var firstFileStream = File.OpenText(path1);
            using var secondFileStream = File.OpenText(path2);

            var resultFileName = $"{_directory}{finalName}-sorted.txt";
            using var resultFile = new StreamWriter(resultFileName);

            var currentLine1 = JsonSerializer.Deserialize<Line>(firstFileStream.ReadLine());
            var currentLine2 = JsonSerializer.Deserialize<Line>(secondFileStream.ReadLine());

            while (!secondFileStream.EndOfStream && !firstFileStream.EndOfStream)
            {
                var compareRes = currentLine1.CompareTo(currentLine2);
                if (compareRes < 0)
                {
                    resultFile.WriteLine($"{currentLine1.SerialNumber}. {currentLine1.CompanyName}");
                    if (!firstFileStream.EndOfStream)
                    {
                        currentLine1 = JsonSerializer.Deserialize<Line>(firstFileStream.ReadLine());
                    }
                }
                else
                {
                    resultFile.WriteLine($"{currentLine2.SerialNumber}. {currentLine2.CompanyName}");
                    if (!secondFileStream.EndOfStream)
                    {
                        currentLine2 = JsonSerializer.Deserialize<Line>(secondFileStream.ReadLine());
                    }
                }
            }

            return resultFileName;
        }
    }
}
