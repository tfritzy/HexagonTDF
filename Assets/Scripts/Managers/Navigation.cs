public class Navigation
{
    private HexSide[,] NextMap;

    public Navigation(Vector2Int boardSize)
    {
        NextMap = new HexSide[boardSize.x, boardSize.y];
    }

    private void ReacalculatePath(OverworldSegment segment)
    {

    }
}