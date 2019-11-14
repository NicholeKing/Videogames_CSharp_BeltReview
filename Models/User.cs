using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Videogames.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required(ErrorMessage="Name is required")]
        public string Name {get;set;}

        [Required(ErrorMessage="Email is required")]
        [EmailAddress]
        public string Email {get;set;}

        [Required(ErrorMessage="Password is required")]
        [DataType(DataType.Password)]
        public string Password {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public List<Community> Games {get;set;}

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
    }
}