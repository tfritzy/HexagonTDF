using System.Collections.Generic;

public class Player
{
    public int Gems;
    public Dictionary<int, HashSet<int>> DefeatedFortresses;

    public Player()
    {
        DefeatedFortresses = new Dictionary<int, HashSet<int>>()
        {
            {
                0,
                new HashSet<int>() {1}
            }
        };
    }
}