using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyHorrorMovieApp.Models;
using MyHorrorMovieApp.Services;

namespace MyHorrorMovieApp.Controllers
{
  [Controller]
  public class AuthController : Controller
  {
    private readonly AuthService _authService;
    private readonly MyDbContext _context;

    public AuthController(AuthService authService, MyDbContext context)
    {
      _authService = authService;
      _context = context;
    }

    // [HttpGet("login")]
    public IActionResult Login()
    {
      return View();
    }



    // [HttpPost("login")]
    [HttpPost]
    public IActionResult Login(Login model)
    {
      // Validate the model
      if (!ModelState.IsValid)
      {
        // Model validation failed, return error JSON
        return Json(new { success = false, errorMessage = "Invalid username or password" });
      }

      // Validate the user credentials and retrieve the user ID
      var user = _context.Users.SingleOrDefault(u => u.Username == model.Username && u.Password == model.Password);
      if (user == null)
      {
        // User not found, return error JSON
        return Json(new { success = false, errorMessage = "Invalid username or password" });
      }

      // Generate a JWT token with the user ID
      var token = _authService.GenerateJwtToken(user.Id);

      // Set a cookie containing the token
      Response.Cookies.Append("token", token, new CookieOptions
      {
        HttpOnly = true,
        Secure = true, // Set to true if using HTTPS
        SameSite = SameSiteMode.Lax // Adjust as needed
      });

      // Return success JSON
      return Json(new { success = true, token = token });
    }



    // [HttpPost("register")]
    // public IActionResult Register()
    // {
    //   // Logic for handling user registration
    //   return Ok("Registration successful");
    // }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
      // Clear the "token" cookie by setting its expiration time to a past date
      Response.Cookies.Append("token", "", new CookieOptions
      {
        Expires = DateTime.UtcNow.AddDays(-1),
        HttpOnly = true,
        Secure = true, // Set to true if using HTTPS
        SameSite = SameSiteMode.Strict
      });

      return RedirectToAction("Login", "Auth");
    }
  }
}

