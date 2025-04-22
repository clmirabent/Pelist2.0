using Newtonsoft.Json;

namespace Films.Models.APIModels
{
    public class MoviesResponse
    {
        public int Page { get; set; }
        public List<Movie> Results { get; set; }
    }
    public class Movie
    {
        //DATOS DE API
        public int Id { get; set; }
        public string Title { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        //NUESTROS DATOS

        public decimal Review { get; set; } = 0;
    }
}
