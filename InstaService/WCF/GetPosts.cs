using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using InstagramApiSharp.Classes;

namespace InstaService.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GetPosts" in both code and config file together.
    public class GetPosts : IGetPosts
    {
        public List<string> DoWork(int id)
        {
            var pp = PaginationParameters.MaxPagesToLoad(5);
            var a = Program.instants.First().InstaApi.UserProcessor.GetUserMediaAsync("ikasimoff", pp).GetAwaiter().GetResult();

            return a.Value.Select(i => i.Images[0].URI).ToList();
        }
    }
}
