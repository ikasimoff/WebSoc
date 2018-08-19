using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models.DB
{
    public partial class Messages
    {
        public long Id { get; set; }
        public long DialogId { get; set; }
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
        [Required]
        public string Message { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SeenDate { get; set; }
        public bool IsSeen { get; set; }
    }
}
