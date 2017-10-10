using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    class Mainprg
    {
        static void Main(string[] args)
        {
            string startPoint = "D:\\Худ. Книги";
            var visitor = new FileSystemVisitor(startPoint);
            foreach (var fileSysInfo in visitor.GetFileSystemInfoSequence())
            {
                Console.WriteLine(fileSysInfo);
            }
        }
    }
}
