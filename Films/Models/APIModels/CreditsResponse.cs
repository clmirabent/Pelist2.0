using Newtonsoft.Json;

public class CastMember
{
    public int Id { get; set; }
    public string Name { get; set; }

    [JsonProperty("profile_path")]
    public string ProfilePath { get; set; }
}

public class CreditsResponse
{
    public List<CastMember> Cast { get; set; } = new List<CastMember>();
}
