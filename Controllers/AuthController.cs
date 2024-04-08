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

    [HttpGet("login")] // Allow GET requests to /login
    public IActionResult Login()
    {
      // Return the login view
      return View();
    }



    [HttpPost("login")]
    public IActionResult Login(Login model)
    {
      // Validate the model
      if (!ModelState.IsValid)
      {
        // Model validation failed, return the view with validation errors
        return View(model);
      }

      // Validate the user credentials and retrieve the user ID
      var user = _context.Users.SingleOrDefault(u => u.Username == model.Username && u.Password == model.Password);
      if (user == null)
      {
        ModelState.AddModelError("", "Invalid username or password");
        return View(model);
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

      // Redirect to the "Index" action method of the "Movies" controller after successful login
      return RedirectToAction("Index", "Movies", new { token });
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

