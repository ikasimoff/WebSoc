using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaService
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            var a = new Lib.RepositoryDapper.UserRepository().GetLoginsNoPost(1, 1000);
            sw.Stop();
            
            Console.WriteLine(a.Count);
            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}
