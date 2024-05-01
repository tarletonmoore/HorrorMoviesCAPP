using System;
using System.Linq;
using MyHorrorMovieApp.Models;

namespace MyHorrorMovieApp.Services
{
  public interface IFriendRequestService
  {
    Task AcceptFriendRequest(int friendRequestId);
  }
  public class FriendRequestService : IFriendRequestService
  {
    private readonly MyDbContext _dbContext;

    public FriendRequestService(MyDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task AcceptFriendRequest(int friendRequestId)
    {
      var friendRequest = _dbContext.FriendRequests.FirstOrDefault(fr => fr.Id == friendRequestId);

      if (friendRequest != null && friendRequest.Status == FriendRequestStatus.Pending)
      {
        friendRequest.Status = FriendRequestStatus.Accepted;

        var sender = await _dbContext.Users.FindAsync(friendRequest.SenderId);
        var recipient = await _dbContext.Users.FindAsync(friendRequest.RecipientId);


        Console.WriteLine($"Sender: {(sender != null ? sender.Id.ToString() : "null")}");
        Console.WriteLine($"Sender Name: {(sender != null ? sender.Username.ToString() : "null")}");
        Console.WriteLine($"Recipient: {(recipient != null ? recipient.Id.ToString() : "null")}");


        // Create Friendship records for both users
        // Inside AcceptFriendRequest method
        var friendship1 = new Friendship { UserId = sender.Id, User = sender, FriendId = recipient.Id, Friend = recipient };
        var friendship2 = new Friendship { UserId = recipient.Id, User = recipient, FriendId = sender.Id, Friend = sender };


        // Add the friendships to the context
        _dbContext.Friendships.AddRange(friendship1, friendship2);

        // Remove the friend request from sender's and recipient's lists
        // sender.SentFriendRequests.Remove(friendRequest);
        // recipient.ReceivedFriendRequests.Remove(friendRequest);

        // Save changes to the database
        await _dbContext.SaveChangesAsync();
      }
    }
  }
}
