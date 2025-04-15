namespace Films.Models
{
    public class Review
    {
        public int IdReview { get; set; }
        public int FkIdMovie { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int FkIdUser { get; set; }

        public User User { get; set; }
    }
}
