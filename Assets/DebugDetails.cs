using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugDetails : MonoBehaviour
{
    private Text towerPowerText;
    private Text powerText;

    // Start is called before the first frame update
    void Start()
    {
        this.towerPowerText = this.transform.Find("TowerPower").GetComponent<Text>();
        this.powerText = this.transform.Find("WavePower").GetComponent<Text>();
    }

    float lastUpdateTime;
    void Update()
    {
        if (Time.time > lastUpdateTime + 5f)
        {
            UpdateTowerPower();
            lastUpdateTime = Time.time;
        }
    }

    private void UpdateTowerPower()
    {
        float towerPower = 0;
        foreach (Building building in Managers.Board.Buildings.Values)
        {
            if (building is AttackTower)
            {
                towerPower += building.Power;
            }
        }

        towerPowerText.text = $"Tower Power: {towerPower}";
    }

    private int currentDisplayedWave;
    private float currentDisplayedPower;
    public void InformOfWaveStart(int wave, float powerGiven)
    {
        if (wave > currentDisplayedWave)
        {
            currentDisplayedWave = wave;
            currentDisplayedPower = powerGiven;
        }
        else
        {
            currentDisplayedPower += powerGiven;
        }

        this.powerText.text = $"Wave {currentDisplayedWave}, {(int)currentDisplayedPower} Power";
    }
}
