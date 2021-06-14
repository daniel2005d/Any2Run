using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoExec
{
    class Program
    {
        static void Main(string[] args)
        {
            using(StreamWriter sw = new StreamWriter("c:\\temp\\log.txt"))
            {
                sw.WriteLine(DateTime.Now);
            }
        }
    }
}
