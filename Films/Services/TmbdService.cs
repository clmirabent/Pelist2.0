using System;
using System.IO;
using Films.Models.APIModels;
using Newtonsoft.Json;
namespace Films.Services
{
    public class TmbdService
    {
        public class TmdbService
        {
            private readonly HttpClient _httpClient;
            private readonly string _apiKey = "93ef6162a6b83478d0f6290a5bf1fbe2";

            public TmdbService(IHttpClientFactory httpClientFactory)
            {
                _httpClient = httpClientFactory.CreateClient("TMDb");
            }
            public async Task<List<Movie>> GetMoviesAsync(int totalPages)
            {
                var allMovies = new List<Movie>();

                for (int page = 1; page <= totalPages; page++)
                {
                    var response = await _httpClient.GetAsync($"movie/popular?api_key={_apiKey}&language=es-ES&page={page}");

                    if (!response.IsSuccessStatusCode) break;

                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<MoviesResponse>(json);

                    if (data?.Results != null)
                    {
                        allMovies.AddRange(data.Results);
                    }
                }

                return allMovies;
            }

            public async Task<List<Movie>> GetPopularMovieAsync()
            {
                var response = await _httpClient.GetAsync($"movie/popular?api_key={_apiKey}&language=es-ES&page=1");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<MoviesResponse>(json);

                return data?.Results?.Take(20).ToList() ?? new List<Movie>();
            }

            public async Task<List<Movie>> GetMoviesByGenreAsync(int genreId)
            {
                var response = await _httpClient.GetAsync($"discover/movie?api_key={_apiKey}&language=es-ES&with_genres={genreId}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<MoviesResponse>(json);

                return data?.Results?.Take(20).ToList() ?? new List<Movie>();
            }

            public async Task<List<Genre>> GetGenresAsync()
            {
                var response = await _httpClient.GetAsync($"genre/movie/list?api_key={_apiKey}&language=es-ES");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<GenresResponse>(json);

                return data?.Genres ?? new List<Genre>();
            }

            public async Task<List<People>> GetPopularActorsAsync()
            {
                var response = await _httpClient.GetAsync($"person/popular?api_key={_apiKey}&language=es-ES&page=1");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PeopleResponse>(json);

                return data?.Results ?? new List<People>();
            }

            public async Task<Movie> GetMovieById(int movieId)
            {
                var response = await _httpClient.GetAsync($"movie/{movieId}?api_key={_apiKey}&language=es-ES");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Movie>(json);

                if (data == null)
                    throw new Exception("Error al obtener la película");

                return data;
            }

            public async Task<List<CastMember>> GetMovieCastAsync(int movieId)
            {
                var response = await _httpClient.GetAsync($"movie/{movieId}/credits?api_key={_apiKey}&language=es-ES");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<CreditsResponse>(json);

                return data?.Cast ?? new List<CastMember>();
            }

            public async Task<List<Movie>> SearchMoviesByTitleAsync(string title)
            {
                if (string.IsNullOrWhiteSpace(title))
                    return new List<Movie>();

                var allMovies = new List<Movie>();
                int page = 1;
                int maxMovies = 60; 
                int moviesPerPage = 20;

                while (allMovies.Count < maxMovies)
                {
                    var response = await _httpClient.GetAsync($"search/movie?api_key={_apiKey}&language=es-ES&query={Uri.EscapeDataString(title)}&page={page}");

                    if (!response.IsSuccessStatusCode)
                        break;

                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<MoviesResponse>(json);

                    if (data?.Results == null || data.Results.Count == 0)
                        break;

                    allMovies.AddRange(data.Results);

                    if (data.Results.Count < moviesPerPage)
                        break; 

                    page++;
                }

                return allMovies.Take(maxMovies).ToList();
            }

        }
    }
}
