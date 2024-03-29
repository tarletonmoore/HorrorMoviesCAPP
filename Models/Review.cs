using System.ComponentModel.DataAnnotations;

namespace MyHorrorMovieApp.Models
{
  public class Review
  {
    public int Id { get; set; }
    [Required]
    public int MovieId { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    [MinLength(3)]
    public string Comment { get; set; } = "";

    public Movie Movie { get; set; }
    public User User { get; set; }
  }
}