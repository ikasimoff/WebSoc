using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models.DB
{
    public partial class HightLights
    {
        public long Id { get; set; }
        [Required]
        [StringLength(500)]
        public string HightLight { get; set; }
        public int Count { get; set; }
        public long UserId { get; set; }
    }
}
