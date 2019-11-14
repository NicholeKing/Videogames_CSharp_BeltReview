using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Videogames.Models
{
    public class Game
    {
        [Key]
        public int GameId {get;set;}

        [Required(ErrorMessage="Title is required")]
        public string Title {get;set;}

        [Required(ErrorMessage="Company is required")]
        public string Company {get;set;}

        [Required(ErrorMessage="Year is required")]
        public int Year {get;set;}

        [Required(ErrorMessage="Rating is required")]
        public string Rating {get;set;}

        public int Age {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public List<Community> Owner {get;set;}
    }
}