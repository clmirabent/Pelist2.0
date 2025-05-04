namespace Films.Models.ViewModels;

public class UserProfileViewModel
{
   public User? User { get; set; }
   public IEnumerable<TypeList> TypeLists { get; set; } 
   
   public List<Friend> Friends { get; set; } = new List<Friend>();

   public List<Friend> FriendRequests { get; set; } = new List<Friend>();
   
   public List <Review> Reviews { get; set; } = new List<Review>();
}