using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models.DB
{
    public partial class Posts
    {
        public long Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateTimeInsta { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateTimeParsing { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateTimeShot { get; set; }
        [StringLength(50)]
        public string PostId { get; set; }
        [StringLength(500)]
        public string PostSrc { get; set; }
        [StringLength(500)]
        public string GeoName { get; set; }
        [StringLength(50)]
        public string GeoCord { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public long? UserId { get; set; }
        public bool? IsAds { get; set; }
        public string Markers { get; set; }
        public string Likers { get; set; }
        public string Commentators { get; set; }
        public string TagUsers { get; set; }
        public int? LikersCount { get; set; }
        public int? CommntsCount { get; set; }
    }
}
