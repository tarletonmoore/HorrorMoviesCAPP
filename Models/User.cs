using System.ComponentModel.DataAnnotations;

namespace MyHorrorMovieApp.Models
{
  public class User
  {
    public int Id { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string Username { get; set; }

    [Required]
    [MinLength(5)]
    public string Password { get; set; } = "";

    public bool Admin { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
  }
}

