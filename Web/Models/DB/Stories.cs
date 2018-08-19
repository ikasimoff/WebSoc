using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models.DB
{
    public partial class Stories
    {
        public long Id { get; set; }
        public string Tags { get; set; }
        public string GeoName { get; set; }
        public string Users { get; set; }
        public long UserId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        public string Src { get; set; }
    }
}
