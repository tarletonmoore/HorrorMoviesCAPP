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



namespace MyHorrorMovieApp.Controllers
{

    public class FriendRequestsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IFriendRequestService _friendRequestService;


        public FriendRequestsController(MyDbContext context, IFriendRequestService friendRequestService)
        {
            _context = context;
            _friendRequestService = friendRequestService;

        }

        // GET: FriendRequests
        public async Task<IActionResult> Index()
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
            var user = await _context.Users.FindAsync(userIdInt);

            bool isAdmin = user != null && user.Admin;
            ViewData["IsAdmin"] = isAdmin;
            ViewData["Token"] = token;

            var friendRequests = await _context.FriendRequests
                .Include(f => f.Sender) // Include the sender information
                .Where(f => f.RecipientId == userIdInt && f.Status == FriendRequestStatus.Pending)
                .ToListAsync();

            return View(friendRequests);
        }


        // GET: FriendRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friendRequest = await _context.FriendRequests
                .Include(f => f.Recipient)
                .Include(f => f.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (friendRequest == null)
            {
                return NotFound();
            }

            return View(friendRequest);
        }

        [HttpGet("FriendRequests/HasPendingFriendRequest")]
        public async Task<IActionResult> HasPendingFriendRequest(int profileId)
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

            if (!int.TryParse(userId, out int currentUserId))
            {
                // Handle invalid userId here, such as returning an error response
                return BadRequest("Invalid userId format.");
            }

            ViewData["Token"] = token;

            // Check if there's a pending friend request between the current user and the profile user
            var hasPendingRequest = await _context.FriendRequests
          .AnyAsync(fr =>
              (fr.SenderId == currentUserId && fr.RecipientId == profileId ||
               fr.SenderId == profileId && fr.RecipientId == currentUserId) &&
              fr.Status == FriendRequestStatus.Pending);

            return Json(new { hasPendingRequest });
        }

        [HttpPost]
        public async Task<IActionResult> Create(int recipientId)
        {
            // Get the current user's ID from the claims in the HttpContext
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
            var sender = await _context.Users.FindAsync(userIdInt);
            var recipient = await _context.Users.FindAsync(recipientId);

            // Check if sender or recipient is null
            if (sender == null || recipient == null)
            {
                return BadRequest("Sender or recipient not found.");
            }
            // Create a new FriendRequest object with the current user as the sender
            var friendRequest = new FriendRequest
            {
                SenderId = userIdInt,
                RecipientId = recipientId, // Set the recipient ID
                SentAt = DateTime.Now,
                Status = FriendRequestStatus.Pending, // Set the status to "Pending"
                Sender = sender,
                Recipient = recipient
            };

            // Add the friend request to the context and save changes
            _context.FriendRequests.Add(friendRequest);

            // sender.SentFriendRequests.Add(friendRequest);

            // // Add the friend request to the recipient's received requests
            // recipient.ReceivedFriendRequests.Add(friendRequest);

            await _context.SaveChangesAsync();

            return Ok(new { success = true });
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
            ViewData["RecipientId"] = new SelectList(_context.Users, "Id", "Password", friendRequest.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Password", friendRequest.SenderId);
            return View(friendRequest);
        }



        [HttpPost]
        public async Task<IActionResult> Accept(int friendRequestId)
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
            var friendRequest = await _context.FriendRequests.FindAsync(friendRequestId);
            Console.WriteLine("SENDER ID: {0}", friendRequest.SenderId);
            Console.WriteLine("Recipient ID: {0}", friendRequest.RecipientId);


            if (friendRequest == null)
            {
                return NotFound();
            }

            // Convert userId to integer
            if (!int.TryParse(userId, out int currentUserId))
            {
                // Handle invalid userId here, such as returning an error response
                return BadRequest("Invalid userId format.");
            }
            if (friendRequest.RecipientId != currentUserId)
            {

                return Unauthorized();
            }
            // Call the service method to accept the friend request
            _friendRequestService.AcceptFriendRequest(friendRequestId);

            // Redirect the user to a different page (e.g., home page or friend list)
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            // Get the current user ID from the claims in the HttpContext
            var token = Request.Cookies["token"];

            if (string.IsNullOrEmpty(token))
            {
                // Return an error response or redirect the user to log in
                return RedirectToAction("Login", "Auth");
            }

            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(token);

            // Retrieve the user ID from the token's payload
            var userId = decodedToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

            // Convert userId to integer
            if (!int.TryParse(userId, out int currentUserId))
            {
                // Handle invalid userId here, such as returning an error response
                return BadRequest("Invalid userId format.");
            }

            var friendRequest = await _context.FriendRequests.FindAsync(id);

            // Check if the friend request exists and if the current user is the recipient
            if (friendRequest != null && friendRequest.RecipientId == currentUserId)
            {
                _context.FriendRequests.Remove(friendRequest);
                await _context.SaveChangesAsync();
            }
            else
            {
                return StatusCode(403, new { message = "Not permitted to delete request" });
            }

            return Ok(new { success = true });
        }


        private bool FriendRequestExists(int id)
        {
            return _context.FriendRequests.Any(e => e.Id == id);
        }
    }
}
