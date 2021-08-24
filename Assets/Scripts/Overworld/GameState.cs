﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
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

}
