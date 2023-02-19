using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    private Material mat;
    public int direction = 1;

    void Start()
    {
        this.mat = this.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        Vector2 offset = this.mat.mainTextureOffset;
        offset.x += direction * .2f * Time.deltaTime;
        this.mat.mainTextureOffset = offset;
    }
}
