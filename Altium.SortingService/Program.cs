// See https://aka.ms/new-console-template for more information
using Altium.SortingService.Services;
using System.Diagnostics;

SplittingService splittingService;
var path = "";
var fileName = "";
while (true)
{
    Console.WriteLine("Insert path to the file or Ctrl+C to exit");
    path = Console.ReadLine();
    path = path.Replace("\\", "/");
    Exception ex;
    try
    {
        fileName = path.Split("/").Last();
        path = path.Substring(0, path.Length - fileName.Length);
        splittingService = new SplittingService(path, fileName);
        break;
    }
    catch (Exception _)
    {
        continue;
    }
}

var stopwatch = new Stopwatch();
stopwatch.Start();

var countOfSplittedFiles = await splittingService.SplitFile(fileName);

var sortingService = new SortingService(countOfSplittedFiles, path);

var sortedFiles = await sortingService.Sort();

var mergingService = new MergingService(path);

var result = await mergingService.Merge(sortedFiles, fileName);

stopwatch.Stop();
var ts = stopwatch.Elapsed;
Console.WriteLine("Elapsed Time is {0:00}:{1:00}:{2:00}.{3}",
ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

Console.WriteLine($"You can find sorted file here: {result}");