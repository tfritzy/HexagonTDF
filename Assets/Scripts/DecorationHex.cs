using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationHex : MonoBehaviour
{
    public GameObject[] DecorationOptions;
    public int NumDecorations;
    public float sizeVariance = 0f;

    void Start()
    {
        if (DecorationOptions.Length == 0)
        {
            return;
        }

        Vector3 centerGround = this.transform.position;
        for (int i = 0; i < NumDecorations; i++)
        {
            Vector3 wiggle = Random.insideUnitSphere * .5f;
            wiggle.y = 0;
            GameObject go = Instantiate(DecorationOptions[Random.Range(0, DecorationOptions.Length)], centerGround + wiggle, new Quaternion(), this.transform);
            go.transform.RotateAround(this.transform.position, Vector3.up, Random.Range(0, 360));
            go.transform.localScale = go.transform.localScale * Random.Range(1 - sizeVariance, 1 + sizeVariance);
        }
    }
}