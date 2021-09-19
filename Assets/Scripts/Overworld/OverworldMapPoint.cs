using Newtonsoft.Json;

public class OverworldMapPoint
{
    [JsonProperty("H")]
    public float Height;

    [JsonProperty("B")]
    public Biome Biome;
}