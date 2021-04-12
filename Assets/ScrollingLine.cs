using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingLine : MonoBehaviour
{
    public float Rate;
    private Material Line;
    private Vector2 offset;

    void Start()
    {
        this.Line = this.GetComponent<LineRenderer>().materials[0];
    }

    void Update()
    {
        offset.x += Rate * Time.deltaTime;
        this.Line.mainTextureOffset = offset;
    }
}
