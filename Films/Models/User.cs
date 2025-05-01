using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Films.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? AboutMe { get; set; }
    
    public string? Image { get; set; }

    public virtual ICollection<Friend> FriendFkIdFriendNavigations { get; set; } = new List<Friend>();

    public virtual ICollection<Friend> FriendFkIdUserNavigations { get; set; } = new List<Friend>();

    public virtual ICollection<List> Lists { get; set; } = new List<List>();

    public virtual ICollection<Preference> Preferences { get; set; } = new List<Preference>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    
    [NotMapped]
    public string FriendshipStatus { get; set; }
}
