using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{
    public float ScrollSpeed;

    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        this.mat = this.GetComponent<MeshRenderer>().material;
    }

    float uvOffset = 0f;
    void LateUpdate()
    {
        uvOffset += (ScrollSpeed * Time.deltaTime);
        mat.mainTextureOffset = new Vector2(0, uvOffset);
    }
}
