using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        
        [StringLength(150)]
        public string InstaLink { get; set; }
        public string Fb { get; set; }
        public string Vk { get; set; }
        public string City { get; set; }

        public DateTime BirthDay { get; set; }
        public string Website { get; set; }
        public string Photo { get; set; }
        public string Description { get; set; }
    }
}