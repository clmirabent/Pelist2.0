namespace Films.Models.ViewModels;

public class UserProfileViewModel
{
   public User User { get; set; }
   public IEnumerable<TypeList> TypeLists { get; set; } 
   public List<User> Friends { get; set; }
}