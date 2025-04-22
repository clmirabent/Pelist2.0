using Films.Models.APIModels;

namespace Films.Models.ViewModels
{
    public class HomeViewModel
    {
        public Dictionary<string, List<Movie>> CategorizedMovies { get; set; } = new();

        public List<List> UserMovieLists { get; set; }
    }
}
