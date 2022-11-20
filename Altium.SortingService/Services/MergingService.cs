using Altium.SortingService.Models;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Altium.SortingService.Services
{
    public class MergingService
    {
        private readonly string _directory;

        public MergingService(string directory)
        {
            _directory = directory;
        }

        public async Task<string> Merge(Queue<string> filesToMerge, string fileName)
        {
            var resultName = "";
            while (filesToMerge.Count > 2)
            {
                var firstName = $"{_directory}{filesToMerge.Dequeue()}";
                var secondName = $"{_directory}{filesToMerge.Dequeue()}";

                resultName = await MergeSortedFiles(firstName, secondName);

                File.Delete(firstName);
                File.Delete(secondName);

                filesToMerge.Enqueue(resultName);
            }

            var firstName1 = $"{_directory}{filesToMerge.Dequeue()}";
            var secondName1 = $"{_directory}{filesToMerge.Dequeue()}";

            resultName = await FinalMerge(firstName1, secondName1, fileName);

            File.Delete(firstName1);
            File.Delete(secondName1);

            return resultName;
        }

        public string MergeParallel(ConcurrentQueue<string> filesToMerge, string fileName)
        {
            
            var resultName = "";
            Parallel.For(0, filesToMerge.Count / 2 - 2, (i) =>
            {
                var isFirstSuccess = filesToMerge.TryDequeue(out var firstName);
                var isSecondSuccess = filesToMerge.TryDequeue(out var secondName);
            });
            //while (filesToMerge.Count > 2)
            //{
            //    var firstName = $"{_directory}{filesToMerge.Dequeue()}";
            //    var secondName = $"{_directory}{filesToMerge.Dequeue()}";

            //    resultName = MergeSortedFilesParallel(firstName, secondName);

            //    File.Delete(firstName);
            //    File.Delete(secondName);

            //    filesToMerge.Enqueue(resultName);
            //}

            //var firstName1 = $"{_directory}{filesToMerge.Dequeue()}";
            //var secondName1 = $"{_directory}{filesToMerge.Dequeue()}";

            //resultName = await FinalMerge(firstName1, secondName1, fileName);

            //File.Delete(firstName1);
            //File.Delete(secondName1);

            return resultName;
        }

        private async Task<string> MergeSortedFiles(string path1, string path2)
        {
            var number1 = path1.Split('/').Last().Split('.')[0].Split('-').Last(); // sorted-0
            var number2 = path2.Split('/').Last().Split('.')[0].Split('-').Last(); // sorted-1

            using var firstFileStream = File.OpenText(path1);
            using var secondFileStream = File.OpenText(path2);

            var resultFileName = $"merged-{number1}{number2}.json";
            using var resultFile = new StreamWriter($"{_directory}{resultFileName}");

            var currentLine1 = JsonSerializer.Deserialize<Line>(await firstFileStream.ReadLineAsync());
            var currentLine2 = JsonSerializer.Deserialize<Line>(await secondFileStream.ReadLineAsync());

            while (!secondFileStream.EndOfStream && !firstFileStream.EndOfStream)
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

        private async Task<string> FinalMerge(string path1, string path2, string finalName)
        {
            using var firstFileStream = File.OpenText(path1);
            using var secondFileStream = File.OpenText(path2);

            var resultFileName = $"{_directory}{finalName}-sorted.txt";
            using var resultFile = new StreamWriter(resultFileName);

            var currentLine1 = JsonSerializer.Deserialize<Line>(await firstFileStream.ReadLineAsync());
            var currentLine2 = JsonSerializer.Deserialize<Line>(await secondFileStream.ReadLineAsync());

            while (!secondFileStream.EndOfStream && !firstFileStream.EndOfStream)
            {
                var compareRes = currentLine1.CompareTo(currentLine2);
                if (compareRes < 0)
                {
                    await resultFile.WriteLineAsync($"{currentLine1.SerialNumber}. {currentLine1.CompanyName}");
                    if (!firstFileStream.EndOfStream)
                    {
                        currentLine1 = JsonSerializer.Deserialize<Line>(await firstFileStream.ReadLineAsync());
                    }
                }
                else
                {
                    await resultFile.WriteLineAsync($"{currentLine2.SerialNumber}. {currentLine2.CompanyName}");
                    if (!secondFileStream.EndOfStream)
                    {
                        currentLine2 = JsonSerializer.Deserialize<Line>(await secondFileStream.ReadLineAsync());
                    }
                }
            }

            return resultFileName;
        }

        private string MergeSortedFilesParallel(string path1, string path2)
        {
            var number1 = path1.Split('/').Last().Split('.')[0].Split('-').Last(); // sorted-0
            var number2 = path2.Split('/').Last().Split('.')[0].Split('-').Last(); // sorted-1

            using var firstFileStream = File.OpenText(path1);
            using var secondFileStream = File.OpenText(path2);

            var resultFileName = $"merged-{number1}{number2}.json";
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
