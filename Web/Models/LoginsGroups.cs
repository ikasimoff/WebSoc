using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class LoginsGroups
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long TaskId { get; set; }
        public long? LoginsGroupId { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(50)]
        public string Description { get; set; }
    }
}
