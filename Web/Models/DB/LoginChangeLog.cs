using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models.DB
{
    public partial class LoginChangeLog
    {
        public long Id { get; set; }
        [Required]
        [StringLength(10)]
        public string FollowersCount { get; set; }
        [Required]
        [StringLength(10)]
        public string FollowingsCount { get; set; }
        [Required]
        public string Followers { get; set; }
        [Required]
        public string Followings { get; set; }
        [StringLength(10)]
        public string FollowersCountAdd { get; set; }
        [StringLength(10)]
        public string FollowingsCountAdd { get; set; }
        public string FollowersAdd { get; set; }
        public string FollowingsAdd { get; set; }
        [StringLength(10)]
        public string FollowersCountDel { get; set; }
        [StringLength(10)]
        public string FollowingsCountDel { get; set; }
        public string FollowersDel { get; set; }
        public string FollowingsDel { get; set; }
    }
}
