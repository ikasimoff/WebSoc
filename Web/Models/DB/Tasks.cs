using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models.DB
{
    public partial class Tasks
    {
        public long Id { get; set; }
        [Required]
        [StringLength(500)]
        public string Title { get; set; }
    }
}
