using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveStartButton : MonoBehaviour
{
    public void StartWaveNow()
    {
        foreach (Portal portal in Managers.Map.Portals)
        {
            portal.StartWaveEarly();
        }
    }
}
