using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Models
{
    public class UserAccount
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public Nullable<bool> IsPrivate { get; set; }
        public long PersonalKey { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public string ImageId { get; set; }
        public Nullable<int> FollowersCount { get; set; }
        public string Followers { get; set; }
        public string Followings { get; set; }
        public Nullable<int> FollowingsCount { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsBuisnes { get; set; }
        public Nullable<bool> IsInstagrammer { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public Nullable<System.DateTime> LastParsing { get; set; }
        public string ParsingLogin { get; set; }
        public string Friends { get; set; }
    }

    public class ShortUser
    {
        public long Id { get; set; }
        public string Login { get; set; }
    }
}
