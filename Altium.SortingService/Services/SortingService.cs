using Altium.SortingService.Models;
using Altium.SortingService.Utils;
using System.Text.Json;

namespace Altium.SortingService.Services
{
    public class SortingService
    {
        private readonly int _countOfFiles;
        private readonly string _directory;

        public SortingService(int countOfFiles, string directory)
        {
            _countOfFiles = countOfFiles;
            _directory = directory;
        }

        public async Task<Queue<string>> Sort()
        {
            var files = new Queue<string>();
            for (int i = 0; i < _countOfFiles; i++)
            {
                var filename = $"sorted-{i}.json";

                var unsortedFilePath = $"{_directory}/unsorted-{i}.json";
                var sortedArray = await SortFile(unsortedFilePath);
                await sortedArray.WriteAndSerialize(i, "sorted", _directory);
                File.Delete(unsortedFilePath);

                files.Enqueue(filename);
            }

            return files;
        }

        private async Task<Line[]> SortFile(string path)
        {
            var fileContent = await File.ReadAllLinesAsync(path);
            var deserialized = fileContent.Select(x => JsonSerializer.Deserialize<Line>(x)).ToArray();
            Array.Sort(deserialized);

            return deserialized;
        }
    }
}
