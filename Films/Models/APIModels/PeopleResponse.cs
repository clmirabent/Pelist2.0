using Newtonsoft.Json;
using System.Text.Json;

namespace Films.Models.APIModels
{
    public class PeopleResponse
    {
        public int Page { get; set; }
        [JsonProperty("crew")]
        public List<People> Crew { get; set; }
        public List<People> Results { get; set; }
    }
    public class People
    {
        //DATOS DE API
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("known_for_department")]
        public string Department { get; set; }
        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }
    }
}
