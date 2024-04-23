using System.ComponentModel.DataAnnotations;

namespace MyHorrorMovieApp.Models
{
  public class Favorite
  {

    public int Id { get; set; }
    [Required]
    public int MovieId { get; set; }
    [Required]
    public int UserId { get; set; }

    // Navigation properties
    public Movie Movie { get; set; }
    public User User { get; set; }
  }
}

