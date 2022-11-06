using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altium.SortingService
{
    public class Splitter
    {
        public string WorkingFileName()
        {
            return GetFileName();
        }

        private string GetFileName()
        {
            var directory = new DirectoryInfo("../../../../Altium.FileGenerator/bin/Debug/net6.0");
            var myFile = directory.GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault(x => x.Name.Contains("file-"));

            return myFile.Name;
        }
    }
}
