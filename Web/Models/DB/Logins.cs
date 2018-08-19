using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models.DB
{
    public partial class Logins
    {
        public long Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Login { get; set; }
        [Required]
        [StringLength(50)]
        public string Password { get; set; }
        public long? AccountId { get; set; }
        public bool IsBlocked { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUse { get; set; }
        public bool UseForParsing { get; set; }
        public bool? UseProxy { get; set; }
        [StringLength(50)]
        public string ProxyIp { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdate { get; set; }
    }
}
