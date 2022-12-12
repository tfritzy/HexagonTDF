using Newtonsoft.Json;

public class OverworldMapPoint
{
    [JsonProperty("H")]
    public int Height;

    [JsonProperty("B")]
    public Biome Biome;
}