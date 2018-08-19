using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models.DB
{
    public partial class Users
    {
        public long Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Login { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsBuisnes { get; set; }
        public bool? IsGroup { get; set; }
        public bool? IsAds { get; set; }
        public bool? IsInstagrammer { get; set; }
        [Required]
        [Column("PK")]
        [StringLength(50)]
        public string Pk { get; set; }
        [Required]
        [StringLength(500)]
        public string FullName { get; set; }
        [StringLength(500)]
        public string Image { get; set; }
        [StringLength(10)]
        public string ImageId { get; set; }
        [StringLength(10)]
        public string FollowersCount { get; set; }
        [StringLength(10)]
        public string FollowingsCount { get; set; }
        public string Followers { get; set; }
        public string Followings { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastParsings { get; set; }
        [StringLength(50)]
        public string ParsingLogin { get; set; }
        public string Friends { get; set; }
        [StringLength(250)]
        public string PublicEmail { get; set; }
        [StringLength(50)]
        public string PublicPhone { get; set; }
        public int? Gender { get; set; }
        public int? Old { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Birthdate { get; set; }
        public int? HasChildValue { get; set; }
        public int? HasPetsValue { get; set; }
        [StringLength(50)]
        public string FindIn { get; set; }
    }
}
