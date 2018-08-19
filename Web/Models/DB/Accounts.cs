using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models.DB
{
    public partial class Accounts
    {
        public long Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Login { get; set; }
    }
}
