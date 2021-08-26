using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private float birthTime;
    private bool collected;
    void Start()
    {
        birthTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (collected)
        {
            return;
        }

        if (Time.time > birthTime + 1f)
        {
            Collect();
        }
    }

    private void Collect()
    {
        collected = true;
        GameState.Player.Gems += 1;
        ResourceNumber resourceNumber = Instantiate(Prefabs.ResourceNumber, Managers.Canvas).GetComponent<ResourceNumber>();
        resourceNumber.SetValue(1, this.gameObject, ResourceType.Gem);
        Destroy(this.gameObject);
    }
}
