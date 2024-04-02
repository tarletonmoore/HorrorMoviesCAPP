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

    public AuthController(AuthService authService)
    {
      _authService = authService;
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
      Console.WriteLine("ModelState IsValid: " + ModelState.IsValid);
      foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
      {
        Console.WriteLine("Error: " + error.ErrorMessage);
      }

      if (!ModelState.IsValid)
      {
        // Model validation failed, return the view with validation errors
        return View(model);
      }

      if (!_authService.IsValidCredentials(model.Username, model.Password))
      {
        ModelState.AddModelError("", "Invalid username or password");
        return View(model);
      }

      // Generate a JWT token
      var token = _authService.GenerateJwtToken(model.Username);
      // Set a cookie containing the token
      Response.Cookies.Append("token", token, new CookieOptions
      {
        HttpOnly = true,
        Secure = true, // Set to true if using HTTPS
        SameSite = SameSiteMode.Strict // Adjust as needed
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

    // [HttpPost("logout")]
    // public IActionResult Logout()
    // {
    //   // Logic for handling user logout
    //   return Ok("Logout successful");
    // }
  }
}

