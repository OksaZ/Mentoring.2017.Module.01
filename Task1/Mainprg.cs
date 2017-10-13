using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    class Mainprg
    {
        static void Main(string[] args)
        {
            string startPoint = "D:\\LINQPad5";
            var visitor = new FileSystemVisitor(startPoint, (info) => info.Name.Length < 12);
            visitor.Start += (s, e) =>
            {
                Console.WriteLine("Iteration started");
            };

            visitor.Finish += (s, e) =>
            {
                Console.WriteLine("Iteration finished");
            };

            visitor.FileFinded += (s, e) =>
            {
                Console.WriteLine("\tFounded file: " + e.FindedItem.Name);
            };

            visitor.DirectoryFinded += (s, e) =>
            {
                //Console.WriteLine("\tFounded directory: " + e.Directory.Name);
            };

            visitor.FilteredFileFinded += (s, e) =>
            {
                Console.WriteLine("Founded filtered file: " + e.FindedItem.Name);
            };

            visitor.FilteredDirectoryFinded += (s, e) =>
            {
                //Console.WriteLine("Founded filtered directory: " + e.FilteredDirectory.Name);
            };

            foreach (var fileSysInfo in visitor.GetFileSystemInfoSequence())
            {
                Console.WriteLine(fileSysInfo);
            }
        }
    }
}
