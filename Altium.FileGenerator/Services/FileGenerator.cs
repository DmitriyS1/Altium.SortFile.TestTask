namespace Altium.FileGenerator.Services
{
    /// <summary>
    /// Generates files
    /// </summary>
    public class FileGenerator
    {
        private static readonly Random random = new Random();

        public string Generate(long size)
        {
            var names = UploadNames(100);
            var fileName = $"file-{size}.txt";
            using var file = File.CreateText(fileName);
            while(size >= 0)
            {
                var number = random.Next(0, 10000);
                var index = random.Next(0, names.Count);
                var line = $"{number}. {names[index]}";
                
                file.WriteLine(line);
                size -= line.Length;
            }

            return $"{Directory.GetCurrentDirectory()}/{fileName}";
        }

        private List<string> UploadNames(int? count = 100)
        {
            var names = new List<string>();
            using var nasdaqFile = File.OpenText("nasdaq_screener.csv");
            while (!nasdaqFile.EndOfStream || count >= 0)
            {
                var name = nasdaqFile.ReadLine().Split(',')[1];
                names.Add(name);
                if (count != null)
                {
                    count--;
                }
            }

            return names;
        }
    }
}
