using Altium.SortingService.Models;
using System.Text.Json;

namespace Altium.SortingService.Utils
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Writes array to the file with serialization
        /// </summary>
        /// <param name="lines">Array with data</param>
        /// <param name="fileNumber">Number of the file</param>
        /// <param name="sorted">String to show if file is sorted</param>
        /// <param name="directory">Directory where to store the file</param>
        public static async Task WriteAndSerialize(this IEnumerable<Line> lines, int fileNumber, string sorted, string directory)
        {
            await File.WriteAllLinesAsync($"{directory}/{sorted}-{fileNumber}.json", lines.Select(x => JsonSerializer.Serialize(x)));
        }
    }
}
