using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Building : MonoBehaviour
{
    public abstract Sprite Icon { get; }
    public abstract BuildingType Type { get; }
}
