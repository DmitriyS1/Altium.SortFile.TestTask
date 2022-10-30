using Altium.FileGenerator.Models;

Console.WriteLine("File generator");
Console.WriteLine("Created file will have next format: \"Number. String\" for each line, i.e. 134. Apple");

var isCorrectInput = false;
while (!isCorrectInput)
{
    Console.WriteLine("Enter size of the file in the format 100M - for 100 MB file, 10G - for 10 GB file");

    var sizeString = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(sizeString))
    {
        ErrorMessages.PrintFormatError();
        continue;
    }

    var lastChar = sizeString[sizeString.Length - 1];
    if (lastChar != 'M' || lastChar != 'G')
    {
        ErrorMessages.PrintFormatError();
        continue;
    }

    var stringNumber = sizeString.Remove(sizeString.Length - 1, 1);
    var success = int.TryParse(stringNumber, out var number);
    if (!success)
    {
        ErrorMessages.PrintFormatError();
        continue;
    }

    var count = 0L;
    switch (lastChar)
    {
        case 'M':
            count = 512 * number;
            break;
        default:
            count = 512 * 1024 * number;
            break;
    }


}

