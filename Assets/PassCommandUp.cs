using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassCommandUp : MonoBehaviour, Interactable
{
    private Hexagon hexagon;

    void Start()
    {
        this.hexagon = this.transform.parent.GetComponent<Hexagon>();
    }

    public void Interact()
    {
        hexagon.Interact();
    }
}
