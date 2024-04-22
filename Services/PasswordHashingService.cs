﻿using BCrypt.Net;

namespace MyHorrorMovieApp.Services
{
  public class PasswordHashingService
  {
    public string HashPassword(string password)
    {
      return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
      return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
  }
}

