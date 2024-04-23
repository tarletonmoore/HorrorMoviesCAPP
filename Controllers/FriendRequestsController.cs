using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyHorrorMovieApp.Models;

namespace MyHorrorMovieApp.Controllers
{
    public class FriendRequestsController : Controller
    {
        private readonly MyDbContext _context;

        public FriendRequestsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: FriendRequests
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.FriendRequests.Include(f => f.Receiver).Include(f => f.Sender);
            return View(await myDbContext.ToListAsync());
        }

        // GET: FriendRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friendRequest = await _context.FriendRequests
                .Include(f => f.Receiver)
                .Include(f => f.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (friendRequest == null)
            {
                return NotFound();
            }

            return View(friendRequest);
        }

        // GET: FriendRequests/Create
        public IActionResult Create()
        {
            ViewData["ReceiverId"] = new SelectList(_context.Users, "Id", "Password");
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Password");
            return View();
        }

        // POST: FriendRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SenderId,ReceiverId,Status")] FriendRequest friendRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(friendRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReceiverId"] = new SelectList(_context.Users, "Id", "Password", friendRequest.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Password", friendRequest.SenderId);
            return View(friendRequest);
        }

        // GET: FriendRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friendRequest = await _context.FriendRequests.FindAsync(id);
            if (friendRequest == null)
            {
                return NotFound();
            }
            ViewData["ReceiverId"] = new SelectList(_context.Users, "Id", "Password", friendRequest.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Password", friendRequest.SenderId);
            return View(friendRequest);
        }

        // POST: FriendRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SenderId,ReceiverId,Status")] FriendRequest friendRequest)
        {
            if (id != friendRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(friendRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FriendRequestExists(friendRequest.Id))
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
            ViewData["ReceiverId"] = new SelectList(_context.Users, "Id", "Password", friendRequest.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Password", friendRequest.SenderId);
            return View(friendRequest);
        }

        // GET: FriendRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friendRequest = await _context.FriendRequests
                .Include(f => f.Receiver)
                .Include(f => f.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (friendRequest == null)
            {
                return NotFound();
            }

            return View(friendRequest);
        }

        // POST: FriendRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var friendRequest = await _context.FriendRequests.FindAsync(id);
            if (friendRequest != null)
            {
                _context.FriendRequests.Remove(friendRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FriendRequestExists(int id)
        {
            return _context.FriendRequests.Any(e => e.Id == id);
        }
    }
}
