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

            public async Task<List<Movie>> GetPopularMovieTitlesAsync()
            {
                var response = await _httpClient.GetAsync($"movie/popular?api_key={_apiKey}&language=es-ES&page=1");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PopularMoviesResponse>(json);

                return data?.Results?.Take(10).ToList() ?? new List<Movie>();
            }
        }

    }
}
