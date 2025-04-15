namespace Films.Models
{
    public class Preference
    {
        public int IdPreference { get; set; }
        public int FkIdUser { get; set; }
        public int FkIdCategory { get; set; }

        public User User { get; set; }
    }
}
