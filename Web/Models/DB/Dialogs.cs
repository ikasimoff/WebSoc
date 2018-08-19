using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models.DB
{
    public partial class Dialogs
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long LoginId { get; set; }
        public long ToUserId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
    }
}
