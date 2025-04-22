using Films.Models.APIModels;

namespace Films.Models.ViewModels
{
    public class HomeViewModel
    {
        public Dictionary<string, List<Movie>> CategorizedMovies { get; set; } = new();

        public List<List> UserMovieLists { get; set; }

        public List<Genre> Genres { get; set; }

        public List<People> Actors { get; set; }
    }
}
