using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    private static Player player;
    public static Player Player
    {
        get
        {
            if (player == null)
            {
                player = new Player();
            }
            return player;
        }
        set
        {
            player = value;
        }
    }

    private static float levelPowerMultiplier;
    public static float LevelPowerMultiplier
    {
        get
        {
            return Mathf.Max(levelPowerMultiplier, 1);
        }
        set
        {
            levelPowerMultiplier = value;
        }
    }

    public static OverworldSegment SelectedSegment;
}
