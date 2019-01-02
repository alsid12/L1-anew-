using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1;

namespace Module_2.Advanced
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootDir = @"H:\Al_Sid\Books";

            FileSystemVisitor myVisitor = new FileSystemVisitor(rootDir);
            myVisitor.Execute();

            foreach (var foundFile in myVisitor)
            {
                Console.WriteLine(foundFile);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
