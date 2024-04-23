using System.ComponentModel.DataAnnotations;

namespace MyHorrorMovieApp.Models
{
  public class User
  {
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be at least 3 characters.")]

    public string Username { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Password must be at least 5 characters.")]

    public string Password { get; set; } = "";

    public bool Admin { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<FriendRequest> SentFriendRequests { get; set; }
    public ICollection<FriendRequest> ReceivedFriendRequests { get; set; }
    public ICollection<User> Friends { get; set; }


  }
}

