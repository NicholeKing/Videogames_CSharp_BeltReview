using System;
using System.ComponentModel.DataAnnotations;

namespace Videogames.Models
{
    public class Community 
    {

        [Key]
        public int CommunityId {get;set;}
        public int UserId {get;set;}
        public int GameId {get;set;}
        public User User {get;set;}
        public Game Game {get;set;}
    }
}