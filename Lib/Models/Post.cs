using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Models
{
    public class Post
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> DateTimeInsta { get; set; }
        public Nullable<System.DateTime> DateTimeParsing { get; set; }
        public Nullable<System.DateTime> DateTimeShot { get; set; }
        public string PostIdInsta { get; set; }
        public string PostSrcs { get; set; }
        public string GeoName { get; set; }
        public string GeoCoord { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public Nullable<long> UserId { get; set; }
        public Nullable<bool> IsAds { get; set; }
        public string Markers { get; set; }
        public string Likers { get; set; }
    }
}
