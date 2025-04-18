namespace Films.Models
{
    public class Friend
    {
        public int IdFriend { get; set; }
        public int FkIdUser { get; set; }
        public int FkIdFriend { get; set; }
        public bool PendingFriend { get; set; }

        public User User { get; set; }
        public User UserFriend { get; set; }
    }
}
