using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionCallRelay : MonoBehaviour
{
    public GameObject GameObjectToRelayTo;

    public void OnParticleCollision(GameObject other)
    {
        GameObjectToRelayTo.SendMessage("TriggerParticleCollision", other);
    }

    public void DealDamage()
    {
        GameObjectToRelayTo.SendMessage("DealDamage");
    }
}
