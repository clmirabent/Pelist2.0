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
        public string Overview { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }
        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; } // Agregado para la vista de detalle

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("genres")]
        public List<Genre> Genres { get; set; } = new List<Genre>();

        // Actores
        public List<People> Persons { get; set; } = new List<People>();

        //NUESTROS DATOS
        public decimal Review { get; set; } = 0;
    }
}
