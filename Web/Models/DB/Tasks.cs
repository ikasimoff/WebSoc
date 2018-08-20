using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models.DB
{
    public partial class Tasks
    {
        public long Id { get; set; }
        public long AccountId { get; set; }

        [Required]
        [StringLength(500)]
        public string Title { get; set; }

        public string DirectText { get; set; }
        public string CommentText { get; set; }

        [Required]
        public string SQL { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Start { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Stop { get; set; }

        public string SelectedIds { get; set; }
        public int SelectedCount { get; set; }
        public string SelectedIdsDirect { get; set; }
        public string SelectedIdsDirectLikes { get; set; }
        public string SelectedIdsFollow { get; set; }
        public string SelectedIdsComment { get; set; }
        public string CommentedPostId { get; set; }
    }
}
