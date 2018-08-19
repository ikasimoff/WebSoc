using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models.DB
{
    public partial class Markers
    {
        public long Id { get; set; }
        [Required]
        [StringLength(500)]
        public string Marker { get; set; }
    }
}
