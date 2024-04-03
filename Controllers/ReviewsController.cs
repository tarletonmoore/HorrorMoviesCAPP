using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyHorrorMovieApp.Models;
using System.Security.Claims;


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
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Image");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password");
            return View();
        }


        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("Id,MovieId,Comment")] Review review)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         // Get the current user's ID
        //         string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //         // Assign the current user's ID to the review
        //         review.UserId = int.Parse(currentUserId);

        //         _context.Add(review);
        //         await _context.SaveChangesAsync();

        //         // Redirect back to the movies index page
        //         return RedirectToAction("Index", "Movies");
        //     }
        //     // If model state is not valid, return the view with errors
        //     return View(review);
        // }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MovieId,Comment")] Review review)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated
                // For example, redirect to the login page
                return RedirectToAction("Login", "Account");
            }

            // Retrieve the current user's ID
            string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (ModelState.IsValid)
            {
                // Assign the current user's ID to the review
                review.UserId = int.Parse(currentUserId);

                // Add the review to the DbContext
                _context.Reviews.Add(review);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Redirect back to the movies index page
                return RedirectToAction("Index", "Movies");
            }

            // Log model state errors
            foreach (var modelStateEntry in ModelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }


            // If model state is not valid, return the view with errors
            return View(review);
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

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}
