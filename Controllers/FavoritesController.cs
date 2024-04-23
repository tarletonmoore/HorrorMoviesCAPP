using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyHorrorMovieApp.Models;
using System.IdentityModel.Tokens.Jwt;


namespace MyHorrorMovieApp.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly MyDbContext _context;

        public FavoritesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Favorites
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Favorites.Include(f => f.Movie).Include(f => f.User);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Favorites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favorite = await _context.Favorites
                .Include(f => f.Movie)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (favorite == null)
            {
                return NotFound();
            }

            return View(favorite);
        }

        // GET: Favorites/Create
        public IActionResult Create()
        {
            var token = Request.Cookies["token"];


            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");

            }


            return RedirectToAction("Index", "Movies");
        }

        // POST: Favorites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,UserId")] Favorite favorite)
        {

            var token = Request.Cookies["token"];


            if (string.IsNullOrEmpty(token))
            {
                // return an error response or redirect the user to log in
                return RedirectToAction("Login", "Auth");

            }

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

            favorite.UserId = userIdInt;

            // Retrieve the movie associated with the review
            var movie = await _context.Movies.FindAsync(favorite.MovieId);

            if (movie == null)
            {
                // Return a JSON response with an error message
                return Json(new { success = false, message = "Movie not found" });
            }

            // Assign the movie to the review
            favorite.Movie = movie;

            // Add the review to the DbContext
            _context.Favorites.Add(favorite);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a JSON response indicating success
            return Json(new { success = true });
        }

        // GET: Favorites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favorite = await _context.Favorites.FindAsync(id);
            if (favorite == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Image", favorite.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password", favorite.UserId);
            return View(favorite);
        }

        // POST: Favorites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,UserId")] Favorite favorite)
        {
            if (id != favorite.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favorite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavoriteExists(favorite.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Image", favorite.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password", favorite.UserId);
            return View(favorite);
        }

        // GET: Favorites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var token = Request.Cookies["token"];


            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");

            }


            return RedirectToAction("Index", "Movies");

        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var token = Request.Cookies["token"];


            if (string.IsNullOrEmpty(token))
            {
                // return an error response or redirect the user to log in
                return RedirectToAction("Login", "Auth");

            }
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

            // Query the database to retrieve the user record based on the user ID
            var user = await _context.Users.FindAsync(userIdInt);
            var favorite = await _context.Favorites.FindAsync(id);


            if (favorite.UserId == userIdInt)
            {
                try
                {

                    if (favorite == null)
                    {
                        return Json(new { success = false, message = "Favorite not found" });
                    }

                    _context.Favorites.Remove(favorite);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception occurred while deleting favorite:", ex);
                    return StatusCode(500, "An error occurred while deleting favorite.");
                }
            }
            else
            {
                return StatusCode(403, new { message = "Not permitted to delete favorite" });
            }
        }

        private bool FavoriteExists(int id)
        {
            return _context.Favorites.Any(e => e.UserId == id);
        }
    }
}
