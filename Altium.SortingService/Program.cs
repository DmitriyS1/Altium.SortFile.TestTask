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
        splittingService = new SplittingService(path, fileName, 2500000); // 5M -> 390 MB
        break;
    }
    catch (Exception _)
    {
        continue;
    }
}

var stopwatchFull = new Stopwatch();
stopwatchFull.Start();

var countOfSplittedFiles = await splittingService.SplitFile(fileName);
var sortedFiles = new Queue<string>();
for (var i = 0; i < countOfSplittedFiles; i++)
{
    sortedFiles.Enqueue($"sorted-{i}.txt");
}

// Mesuring Merging operation
var mergingService = new MergingService(path);
var stopwatchMerge = new Stopwatch();
stopwatchMerge.Start();

var result = await mergingService.Merge(sortedFiles, fileName);
// var result = mergingService.MergeParallel(sortedFiles, fileName);

stopwatchMerge.Stop();
var tsMerge = stopwatchMerge.Elapsed;
Console.WriteLine("Merging Elapsed Time is {0:00}:{1:00}:{2:00}.{3}",
tsMerge.Hours, tsMerge.Minutes, tsMerge.Seconds, tsMerge.Milliseconds);

stopwatchFull.Stop();
var ts = stopwatchFull.Elapsed;
Console.WriteLine("Full Elapsed Time is {0:00}:{1:00}:{2:00}.{3}",
ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

Console.WriteLine($"You can find sorted file here: {result}");