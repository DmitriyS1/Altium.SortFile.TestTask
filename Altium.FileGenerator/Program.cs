﻿using Altium.FileGenerator.Services;

Console.WriteLine("File generator");
Console.WriteLine("Created file will have the next format: \"Number. String\" for each line, i.e. 134. Apple");

var size = MenuService.GetFileSize();

var fileGenerator = new FileGenerator();
var filePath = fileGenerator.Generate(size);
Console.WriteLine($"File created under the path: {filePath}");

// D:\Developing\C#\Altium.SortFile.TestTask\Altium.FileGenerator\bin\Debug\net6.0/file-104857600.txt