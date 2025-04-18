namespace Films.Models
{
    public class MovieReview
    {
        public int IdMoviewReview { get; set; }
        public int FkIdMovie { get; set; }
        public float AverageRating { get; set; }
    }
}
