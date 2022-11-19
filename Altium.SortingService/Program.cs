// See https://aka.ms/new-console-template for more information
using Altium.SortingService.Services;


SplittingService splittingService;
var path = "";
var fileName = "";
while (true)
{
    Console.WriteLine("Insert path to the file or Ctrl+C to exit");
    path = Console.ReadLine();
    path = path.Replace("\\", "/");

    try
    {
        fileName = path.Split("/").Last();
        path = path.Substring(0, path.Length - fileName.Length);
        splittingService = new SplittingService(path, fileName);
        break;
    }
    catch (Exception ex)
    {
        continue;
    }
}


var countOfSplittedFiles = await splittingService.SplitFile(fileName);

var sortingService = new SortingService(countOfSplittedFiles, path);

var sortedFiles = await sortingService.Sort();
