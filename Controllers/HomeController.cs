using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Videogames.Models;

namespace Videogames.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("RegCheck")]
        public IActionResult RegCheck(User u)
        {
            if(ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                u.Password = Hasher.HashPassword(u, u.Password);
                dbContext.Add(u);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UID", u.UserId);
                return Redirect("Dashboard");
            }
            return View("Index");
        }

        [HttpPost("LogCheck")]
        public IActionResult LogCheck(LUser l)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == l.LEmail);
                // If no user exists with provided email
                if(userInDb == null)
                {
                // Add an error to ModelState and return to View!
                    ModelState.AddModelError("LEmail", "Invalid Email/Password");
                    return View("Index");
                }
            
                // Initialize hasher object
                var hasher = new PasswordHasher<LUser>();
            
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(l, userInDb.Password, l.LPassword);
            
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    ModelState.AddModelError("LEmail", "Invalid Email/Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UID", userInDb.UserId);
                return Redirect("Dashboard");
            }
            return View("Index");
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            int? Sess = HttpContext.Session.GetInt32("UID");
            if(Sess == null)
            {
                return Redirect("/");
            }
            User loggedIn = dbContext.Users.FirstOrDefault(u => u.UserId == (int)Sess);
            ViewBag.User = loggedIn;

            User YourGames = dbContext.Users.Include(s => s.Games).ThenInclude(v => v.Game).FirstOrDefault(u => u.UserId == (int)Sess);
            foreach(var h in YourGames.Games)
            {
                int year = DateTime.Now.Year - h.Game.Year;
                h.Game.Age = year;
            }
            ViewBag.YourGames = YourGames;

            //Query for games we haven't joined
            var inClub = YourGames.Games.Select(x=>x.Game);

            //All the games
            ViewBag.AllGames = dbContext.Games.Except(inClub).OrderByDescending(a => a.Year).ToList();
            foreach(var g in ViewBag.AllGames)
            {
                int year = DateTime.Now.Year - g.Year;
                g.Age = year;
            }
            return View("Dashboard", ViewBag.User);
        }

        //Page to create a videogame on
        [HttpGet("AddGame")]
        public IActionResult AddGame()
        {
            int? Sess = HttpContext.Session.GetInt32("UID");
            if(Sess == null)
            {
                return Redirect("/");
            }
            return View("AddGame");
        }

        //Post for creating videogame
        [HttpPost("Add")]
        public IActionResult Add(Game g)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Games.Any(v => v.Title == g.Title))
                {
                    ModelState.AddModelError("Title", "Title is already created!");
                    return View("AddGame");
                } else {
                    dbContext.Add(g);
                    dbContext.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
            }
            return View("AddGame");
        }

        //Page for reading a single videogame
        [HttpGet("OneGame/{GameId}")]
        public IActionResult OneGame(int GameId)
        {
            int? Sess = HttpContext.Session.GetInt32("UID");
            if(Sess == null)
            {
                return Redirect("/");
            }
            ViewBag.TheGame = dbContext.Games.FirstOrDefault(c => c.GameId == GameId);
            return View("OneGame");
        }

        //Page for updating a single videogame
        [HttpGet("UpdateGame/{GameId}")]
        public IActionResult UpdateGame(int GameId)
        {
            int? Sess = HttpContext.Session.GetInt32("UID");
            if(Sess == null)
            {
                return Redirect("/");
            }
            ViewBag.TheGame = dbContext.Games.FirstOrDefault(g => g.GameId == GameId);
            return View("UpdateGame");
        }

        //Post for updating the videogame
        [HttpPost("Update/{GameId}")]
        public IActionResult Update(Game g)
        {
            if(ModelState.IsValid)
            {
                Game Orig = dbContext.Games.FirstOrDefault(a => a.GameId == g.GameId);
                Orig.Title = g.Title;
                Orig.Company = g.Company;
                Orig.Year = g.Year;
                Orig.Rating = g.Rating;
                dbContext.SaveChanges();
                return RedirectToAction("OneGame", g);
            }
            ViewBag.TheGame = dbContext.Games.FirstOrDefault(c => c.GameId == g.GameId);
            return View("UpdateGame", g);
        }

        //Get for deleting a single videogame
        [HttpGet("DeleteGame/{GameId}")]
        public IActionResult DeleteGame(int GameId)
        {
            Game DeletedGame = dbContext.Games.FirstOrDefault(g => g.GameId == GameId);
            dbContext.Remove(DeletedGame);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("DashboardRedir")]
        public IActionResult DashboardRedir()
        {
            return RedirectToAction("Dashboard");
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        [HttpGet("Join/{UserId}/{GameId}")]
        public IActionResult Join(int UserId, int GameId)
        {
            Community JoinFan = new Community();
            JoinFan.UserId = UserId;
            JoinFan.GameId = GameId;
            dbContext.Add(JoinFan);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("Unjoin/{UserId}/{GameId}")]
        public IActionResult Unjoin(int UserId, int GameId)
        {
            var Unjoin = dbContext.Communities.Where(a => a.UserId == UserId).FirstOrDefault(b => b.GameId == GameId);
            dbContext.Remove(Unjoin);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}
