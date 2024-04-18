using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyHorrorMovieApp.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace MyHorrorMovieApp.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MyDbContext _context;

        public MoviesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Movies

        public async Task<IActionResult> Index()
        {
            // Retrieve the token from the query string
            string token = HttpContext.Request.Query["token"];

            if (string.IsNullOrEmpty(token))
            {
                // return an error response or redirect the user to log in
                return BadRequest("JWT token is missing or invalid.");
            }

            try
            {
                // Parse and validate the token
                var handler = new JwtSecurityTokenHandler();
                var decodedToken = handler.ReadJwtToken(token);

                // Retrieve the user ID from the token's payload
                var userId = decodedToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

                // Set the user ID in ViewData to make it available in the view
                ViewData["UserId"] = userId;
                ViewData["Token"] = token;
                Console.WriteLine("Index Token!!!!!!!!!: {0}", token);

                // Retrieve movies with reviews and users asynchronously
                var moviesWithReviewsAndUsers = await _context.Movies
                    .Include(m => m.Reviews)
                        .ThenInclude(r => r.User)
                    .ToListAsync();

                // Return the view with movies data
                return View(moviesWithReviewsAndUsers);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during token parsing or validation
                // For example, return an error response or log the exception
                return StatusCode(500, "An error occurred while processing the JWT token.");
            }
        }



        // // GET: Movies/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            string token = HttpContext.Request.Query["token"];

            ViewData["Token"] = token;
            // Console.WriteLine("Create Token!!!!!!!!!: {0}", token);

            if (id == null)
            {
                return NotFound();
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

            bool isAdmin = user != null && user.Admin;
            ViewData["IsAdmin"] = isAdmin;

            // Set the user ID in ViewData to make it available in the view
            ViewData["UserId"] = userId;

            // Retrieve the movie from the database based on the id
            var movie = await _context.Movies
                .Include(m => m.Reviews) // Include reviews related to the movie
                    .ThenInclude(r => r.User) // Include users related to each review
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }


        // GET: Movies/Create
        public IActionResult Create()
        {
            string token = HttpContext.Request.Query["token"];

            // Pass the token to the Create view
            ViewData["Token"] = token;
            // Console.WriteLine("Create Token!!!!!!!!!: {0}", token);
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Title,Image,Year,Plot")] Movie movie)
        {
            if (!ModelState.IsValid)
            {

                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage);

                return Json(new { success = false, errors = errors });
            }

            try
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return Ok(new { success = true }); // Return success response
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while saving the movie." }); // Return error response
            }
        }


        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string token = HttpContext.Request.Query["token"];

            // Pass the token to the Create view
            ViewData["Token"] = token;
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Image,Year,Plot")] Movie movie)
        {
            string token = HttpContext.Request.Query["token"];

            // Pass the token to the Create view
            ViewData["Token"] = token;
            // System.Console.WriteLine("TOKEN!!!: {0}", token);
            // System.Console.WriteLine("ID!!!: {0}", id);
            // System.Console.WriteLine("Title!!!: {0}", movie.Title);
            // System.Console.WriteLine("Image!!!: {0}", movie.Image);
            // System.Console.WriteLine("Year!!!: {0}", movie.Year);
            // System.Console.WriteLine("Plot!!!: {0}", movie.Plot);



            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                    return Ok(); // Assuming you return HTTP 200 OK status to indicate success
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return BadRequest(ModelState);
        }


        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // Delete: Movies/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(id);

                if (movie == null)
                {
                    return Json(new { success = false, message = "Movie not found" });
                }

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred while deleting movie:", ex);
                return StatusCode(500, "An error occurred while deleting the movie.");
            }
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
