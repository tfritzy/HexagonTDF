public class Chunk
{
    public Hexagon[,] Hexes = new Hexagon[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE];
    public HexagonMono[,] HexBodies = new HexagonMono[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE];
    public Building[,] Buildings = new Building[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE];

    public HexagonMono GetBody(int x, int y)
    {
        // Allow passing in absolute position.
        if (x >= Constants.CHUNK_SIZE || y >= Constants.CHUNK_SIZE)
        {
            x %= Constants.CHUNK_SIZE;
            y %= Constants.CHUNK_SIZE;
        }

        return HexBodies[x, y];
    }

    public HexagonMono SetBody(int x, int y, HexagonMono value)
    {
        // Allow passing in absolute position.
        if (x >= Constants.CHUNK_SIZE || y >= Constants.CHUNK_SIZE)
        {
            x %= Constants.CHUNK_SIZE;
            y %= Constants.CHUNK_SIZE;
        }

        return HexBodies[x, y] = value;
    }
}