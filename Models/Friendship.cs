namespace MyHorrorMovieApp.Models
{
  public class Friendship
  {
    // User who initiated the friendship
    public int UserId { get; set; }
    public User User { get; set; }

    // User who received the friendship
    public int FriendId { get; set; }
    public User Friend { get; set; }
  }
}

