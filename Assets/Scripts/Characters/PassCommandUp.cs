using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassCommandUp : MonoBehaviour, Interactable
{
    private HexagonMono hexagon;

    void Start()
    {
        this.hexagon = this.transform.parent.GetComponent<HexagonMono>();
    }

    public void Interact()
    {
        hexagon.Interact();
    }
}
