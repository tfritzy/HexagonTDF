using UnityEngine;

public class InstantiatedItem : MonoBehaviour 
{
    public Item Item;

    public void Init(Item item)
    {
        this.Item = item;
    }
}