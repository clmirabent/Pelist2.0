using Newtonsoft.Json;

namespace Films.Models.APIModels
{
    public class PeopleResponse
    {
        public int Page { get; set; }
        public List<People> Results { get; set; }
    }
    public class People
    {
        //DATOS DE API
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
