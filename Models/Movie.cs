using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace MyHorrorMovieApp.Models
{
  public class Movie
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = "";
    [Required]
    public string Image { get; set; } = "";

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

  }
}