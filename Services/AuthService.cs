using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyHorrorMovieApp.Models;

namespace MyHorrorMovieApp.Services
{
  public class AuthService
  {
    private readonly string _secretKey;
    private readonly MyDbContext _context;

    public AuthService(IConfiguration configuration, MyDbContext context)
    {
      _secretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT secret key is not configured.");

      _context = context;
    }



    public bool IsValidCredentials(string username, string password)
    {
      Console.WriteLine("Querying database for user credentials...");

      // Query the database to check if a user with the given username and password exists
      var user = _context.Users.SingleOrDefault(u => u.Username == username && u.Password == password);

      // Return true if a user with the given credentials exists, false otherwise
      return user != null;
    }


    public string GenerateJwtToken(int userId)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_secretKey);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]
          {
            new Claim("userId", userId.ToString()) // Include user ID in the JWT payload
          }),
        Expires = DateTime.UtcNow.AddDays(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        Audience = "http://localhost:5062", // Set the audience claim
        Issuer = "http://localhost:5062" // Set the issuer
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }

  }
}

