using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;


namespace MyHorrorMovieApp.Models
{
  public class User
  {
    public int Id { get; set; }
    [Required]
    public string Username { get; set; } = "";
    [Required]
    [MinLength(5)]
    public string Password { get; set; } = "";

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
  }
}