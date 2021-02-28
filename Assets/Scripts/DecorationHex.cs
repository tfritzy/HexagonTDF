using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationHex : MonoBehaviour
{
    public GameObject[] DecorationOptions;
    public int NumDecorations;

    void Start()
    {
        if (DecorationOptions.Length == 0)
        {
            return;
        }

        Vector3 centerGround = this.transform.position;
        centerGround.y += this.transform.Find("Hex").GetComponent<MeshRenderer>().bounds.extents.y;
        for (int i = 0; i < NumDecorations; i++)
        {
            Vector3 wiggle = Random.insideUnitSphere * .5f;
            wiggle.y = 0;
            GameObject go = Instantiate(DecorationOptions[Random.Range(0, DecorationOptions.Length)], centerGround + wiggle, new Quaternion(), this.transform);
            go.transform.RotateAround(this.transform.position, Vector3.up, Random.Range(0, 360));
        }
    }
}
