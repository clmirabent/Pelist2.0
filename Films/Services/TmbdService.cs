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

            public async Task<List<Movie>> GetPopularMovieAsync()
            {
                var response = await _httpClient.GetAsync($"movie/popular?api_key={_apiKey}&language=es-ES&page=1");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<MoviesResponse>(json);

                return data?.Results?.Take(30).ToList() ?? new List<Movie>();
            }

            public async Task<List<Movie>> GetMoviesByGenreAsync(int genreId)
            {
                var response = await _httpClient.GetAsync($"discover/movie?api_key={_apiKey}&language=es-ES&with_genres={genreId}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<MoviesResponse>(json);

                return data?.Results?.Take(30).ToList() ?? new List<Movie>();
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

            public async Task<List<People>> GetPopularActorsByMovieId(int movieId)
            {
                var response = await _httpClient.GetAsync($"movie/{movieId}/credits?api_key={_apiKey}&language=es-ES");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PeopleResponse>(json);

                return data?.Crew ?? new List<People>();
            }

            public async Task<Movie> GetMovieById(int movieId)
            {
                var response = await _httpClient.GetAsync($"movie/{movieId}?api_key={_apiKey}&language=es-ES");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Movie>(json);

                // Asegurar que data no sea null 
                if (data == null)
                    throw new Exception("Error al obtener la película");

                return data;
            }

        }
    }
}
