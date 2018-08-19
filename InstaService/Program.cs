using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using Lib.Models;
using Lib.RepositoryDapper;
using InstaService.Instagram;
using System.ServiceModel;

namespace InstaService
{
    public static class Program
    {
        public static List<ShortUser> users;
        public static TaskFactory tasks = new TaskFactory();
        public static List<Insta> instants = new List<Insta>();


        public static void Main(string[] args)
        {
            var sw = new Stopwatch();
            //users = new UserRepository().GetLoginsNoPost(1, 200000);

            ServiceHost host = new ServiceHost(typeof(WCF.GetPosts));
            host.Open();
            Console.WriteLine("Service Hosted Sucessfully");
            //Console.Read();

            var irek = new Insta("pavelpetrov141", "петр777");
            irek.StartInstaAsync().GetAwaiter().GetResult();
            //irek.GetPosts("");

            instants.Add(irek);

            #region LoopBenchmarcks
            //sw.Start();
            // 22.5 ms
            //Parallel.ForEach(users, i =>
            //{
            //    Console.WriteLine(i.Login);
            //});

            // 15.8 ms
            //foreach (var i in users)
            //{
            //    Console.WriteLine(i.Login);
            //}

            //14.3 ms
            //for(var j = 0; j<users.Count; j++)
            //{
            //    Console.WriteLine(users[j].Login);
            //}

            //15.3 ms
            //var j = 0;
            //while (j < users.Count-1)
            //{
            //    j++;
            //    Console.WriteLine(users[j].Login);

            //}
            //sw.Stop();
            #endregion
            Console.WriteLine(sw.ElapsedMilliseconds);



            Console.ReadLine();
        }
    }
}
