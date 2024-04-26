using System.ComponentModel.DataAnnotations;

namespace MyHorrorMovieApp.Models
{
  public enum FriendRequestStatus
  {
    Pending,
    Accepted,
    Declined
  }

  public class FriendRequest
  {
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
    public DateTime SentAt { get; set; }
    public FriendRequestStatus Status { get; set; }

    public User Sender { get; set; }
    public User Recipient { get; set; }


  }
}

