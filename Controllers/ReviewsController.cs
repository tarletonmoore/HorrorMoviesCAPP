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
using Newtonsoft.Json;


namespace MyHorrorMovieApp.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly MyDbContext _context;

        public ReviewsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Reviews.Include(r => r.Movie).Include(r => r.User);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Movie)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {

            var token = Request.Cookies["token"];


            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");

            }


            return RedirectToAction("Index", "Movies");
        }


        [HttpPost]
        public async Task<IActionResult> Create(Review review)
        {

            if (review.Comment.Length < 3)
            {
                // Return a JSON response with an error message
                return Json(new { success = false, errors = new[] { "Review must be at least 3 characters long" } });
            }

            // Extract userId from the JWT token payload
            // var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var token = Request.Cookies["token"];


            if (string.IsNullOrEmpty(token))
            {
                // return an error response or redirect the user to log in
                return RedirectToAction("Login", "Auth");

            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            var audience = jsonToken.Audiences.FirstOrDefault();
            if (audience != null)
            {
                Console.WriteLine($"Audience: {audience}");
            }
            else
            {
                Console.WriteLine("Audience claim not found in the token.");
            }

            var userId = Convert.ToInt32(jsonToken.Claims.First(claim => claim.Type == "userId").Value);

            review.UserId = userId;

            // Retrieve the movie associated with the review
            var movie = await _context.Movies.FindAsync(review.MovieId);

            if (movie == null)
            {
                // Return a JSON response with an error message
                return Json(new { success = false, message = "Movie not found" });
            }

            // Assign the movie to the review
            review.Movie = movie;

            // Add the review to the DbContext
            _context.Reviews.Add(review);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a JSON response indicating success
            return Json(new { success = true });
        }


        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Image", review.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password", review.UserId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,UserId,Comment")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
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
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Image", review.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password", review.UserId);
            return View(review);
        }

        // GET: Reviews/Delete/5
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
            try
            {
                // Retrieve token from cookies
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

                // Query the database to retrieve the review based on the review ID and user ID
                var review = await _context.Reviews.FindAsync(id);

                if (review == null)
                {
                    return Json(new { success = false, message = "Review not found" });
                }

                // Check if the review belongs to the user (optional)
                if (review.UserId != userIdInt)
                {
                    return StatusCode(403, new { message = "Not permitted to delete review" });
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred while deleting review:", ex);
                return StatusCode(500, "An error occurred while deleting the review.");
            }
        }



        // Method to check if the current user is the owner of the review
        // ************
        public bool IsCurrentUserReviewOwner(Review review, string userId)
        {
            // System.Console.WriteLine("USER ID!!!!!!", userId);
            if (userId != null && int.TryParse(userId, out int parsedUserId))
            {
                return review.UserId == parsedUserId;
            }
            else
            {
                // Handle the case where userId is null or not a valid integer
                return false;
            }
        }



        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}
