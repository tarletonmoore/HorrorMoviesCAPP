﻿using System.ComponentModel.DataAnnotations;

namespace MyHorrorMovieApp;

public class Login
{
  [Required(ErrorMessage = "Username is required")]

  public string Username { get; set; }
  [Required(ErrorMessage = "Password is required")]

  public string Password { get; set; }
}

