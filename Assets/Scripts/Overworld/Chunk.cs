public class Chunk
{
    private Hexagon[,,] Hexes = new Hexagon[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.HEIGHT_LEVELS];
    private HexagonMono[,,] HexBodies = new HexagonMono[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.HEIGHT_LEVELS];
    private Building[,,] Buildings = new Building[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.HEIGHT_LEVELS];

    private int[,] TopLayerHeights = new int[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE];

    public HexagonMono GetBody(int x, int y)
    {
        // Allow passing in absolute position.
        if (x >= Constants.CHUNK_SIZE || y >= Constants.CHUNK_SIZE)
        {
            x %= Constants.CHUNK_SIZE;
            y %= Constants.CHUNK_SIZE;
        }

        return HexBodies[x, y, TopLayerHeights[x, y]];
    }

    public HexagonMono SetBody(int x, int y, HexagonMono value)
    {
        // Allow passing in absolute position.
        if (x >= Constants.CHUNK_SIZE || y >= Constants.CHUNK_SIZE)
        {
            x %= Constants.CHUNK_SIZE;
            y %= Constants.CHUNK_SIZE;
        }

        return HexBodies[x, y, TopLayerHeights[x, y]] = value;
    }
}