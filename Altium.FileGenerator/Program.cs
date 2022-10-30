using Altium.FileGenerator.Services;

Console.WriteLine("File generator");
Console.WriteLine("Created file will have next format: \"Number. String\" for each line, i.e. 134. Apple");

var size = MenuService.GetFileSize();

var fileGenerator = new FileGenerator();
fileGenerator.Run(size);
