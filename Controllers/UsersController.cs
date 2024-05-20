using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyHorrorMovieApp.Models;
using System.IdentityModel.Tokens.Jwt;
using MyHorrorMovieApp.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MyHorrorMovieApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly MyDbContext _context;
        private readonly PasswordHashingService _passwordHashingService;


        public UsersController(MyDbContext context, PasswordHashingService passwordHashingService)
        {
            _context = context;
            _passwordHashingService = passwordHashingService;

        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details()
        {
            var token = Request.Cookies["token"];


            if (string.IsNullOrEmpty(token))
            {
                // return an error response or redirect the user to log in
                return RedirectToAction("Login", "Auth");

            }

            // Validate and decode the token
            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(token);

            // Retrieve the user ID from the token's payload
            var userId = decodedToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;


            // Convert userId to integer
            if (!int.TryParse(userId, out int userIdInt))
            {
                // Handle invalid userId here, such as returning an error response
                return BadRequest("Invalid userId format.");
            }

            var user = await _context.Users
                .Include(u => u.Favorites)
                    .ThenInclude(f => f.Movie)
                    .Include(u => u.Friendships)
        .ThenInclude(f => f.Friend)
                .SingleOrDefaultAsync(u => u.Id == userIdInt);

            bool areFriends = user.Friendships.Any(f => f.FriendId == userIdInt);
            ViewData["AreFriends"] = areFriends;

            bool isAdmin = user != null && user.Admin;
            ViewData["IsAdmin"] = isAdmin;
            ViewData["Token"] = token;
            ViewData["CurrentUserId"] = userIdInt;
            ViewData["ProfileId"] = user.Id;
            ViewData["ProfilePicture"] = user.ProfilePictureUrl;
            System.Console.WriteLine("Profile Picture {0}", user.ProfilePictureUrl);

            var pendingRequestsCount = await _context.FriendRequests
                  .CountAsync(f => f.RecipientId == userIdInt && f.Status == FriendRequestStatus.Pending);

            ViewData["PendingRequestsCount"] = pendingRequestsCount;


            return View(user);
        }

        public async Task<IActionResult> ViewProfile(string username)
        {
            var token = Request.Cookies["token"];


            if (string.IsNullOrEmpty(token))
            {
                // return an error response or redirect the user to log in
                return RedirectToAction("Login", "Auth");

            }

            // Validate and decode the token
            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(token);

            // Retrieve the user ID from the token's payload
            var userId = decodedToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;


            if (!int.TryParse(userId, out int userIdInt))
            {
                return BadRequest("Invalid userId format.");
            }
            // Retrieve the user based on the provided username
            var user = await _context.Users
                .Include(u => u.Favorites)
                    .ThenInclude(f => f.Movie)
                         .Include(u => u.Friendships)
        .ThenInclude(f => f.Friend)
                .SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return RedirectToAction("Index", "Movies");

            }

            var currentUser = await _context.Users.FindAsync(userIdInt);

            bool areFriends = user.Friendships.Any(f => f.FriendId == userIdInt);
            // System.Console.WriteLine("FRIENDS????? {0}", areFriends);
            ViewData["AreFriends"] = areFriends;

            ViewData["IsAdmin"] = currentUser.Admin;
            ViewData["CurrentUserId"] = userIdInt;
            ViewData["ProfileId"] = user.Id;
            ViewData["ProfilePicture"] = user.ProfilePictureUrl;

            var pendingRequestsCount = await _context.FriendRequests
                  .CountAsync(f => f.RecipientId == userIdInt && f.Status == FriendRequestStatus.Pending);

            ViewData["PendingRequestsCount"] = pendingRequestsCount;


            return View("Details", user);
        }

        public async Task<IActionResult> Search(string username)
        {
            var token = Request.Cookies["token"];


            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");

            }

            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(token);

            var userId = decodedToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;


            if (!int.TryParse(userId, out int userIdInt))
            {
                return BadRequest("Invalid userId format.");
            }

            var currentUser = await _context.Users.FindAsync(userIdInt);

            var pendingRequestsCount = await _context.FriendRequests
                 .CountAsync(f => f.RecipientId == userIdInt && f.Status == FriendRequestStatus.Pending);

            ViewData["PendingRequestsCount"] = pendingRequestsCount;


            ViewData["IsAdmin"] = currentUser.Admin;

            // If no exact match is found, search for closely related users
            var closelyRelatedUsers = await _context.Users
                .Where(u => u.Username.Contains(username))
                .ToListAsync();

            // Pass the closely related users to the view
            return View(closelyRelatedUsers);

        }



        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Check if the username is already taken
                if (_context.Users.Any(u => u.Username == user.Username))
                {
                    // If the username is taken, return an error
                    ModelState.AddModelError("Username", "Username is already taken.");
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, errors = errors });
                }

                // Hash the password before storing it in the database
                string hashedPassword = _passwordHashingService.HashPassword(user.Password);
                user.Password = hashedPassword; // Assign the hashed password to the user object

                // If the username is not taken and the model state is valid, proceed with adding the user
                _context.Add(user);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            else
            {
                // If the model state is not valid, return the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, errors = errors });
            }
        }


        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var token = Request.Cookies["token"];


            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");

            }


            return RedirectToAction("Index", "Movies");
        }

        // POST: Users/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProfilePictureUrl")] User user)
        {
            var token = Request.Cookies["token"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(token);

            var userId = decodedToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

            if (!int.TryParse(userId, out int userIdInt))
            {
                return BadRequest("Invalid userId format.");
            }

            if (user.Id != userIdInt)
            {
                return Forbid();
            }

            var currentUser = await _context.Users.FindAsync(userIdInt);

            if (currentUser == null)
            {
                return NotFound();
            }

            currentUser.ProfilePictureUrl = user.ProfilePictureUrl;

            try
            {
                // Update the user in the database
                _context.Update(currentUser);
                await _context.SaveChangesAsync();
                return Ok(new { success = true }); // Redirect to the user details page
            }
            catch (DbUpdateException)
            {
                // Log the error or handle it accordingly
                return View(user); // Return to the edit view with the user model
            }
        }


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
