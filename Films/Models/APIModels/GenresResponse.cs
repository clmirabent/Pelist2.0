namespace Films.Models.APIModels
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class GenresResponse
    {
        public List<Genre> Genres { get; set; }
    }
}
