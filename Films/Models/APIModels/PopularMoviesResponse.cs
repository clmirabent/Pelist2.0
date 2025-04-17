namespace Films.Models.APIModels
{
    public class PopularMoviesResponse
    {
        public int Page { get; set; }
        public List<Movie> Results { get; set; }
    }
    public class Movie
    {
        public string Title { get; set; }
    }
}
