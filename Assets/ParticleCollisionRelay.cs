using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionRelay : MonoBehaviour
{
    public Building GameObjectToRelayTo;

    void OnParticleCollision(GameObject other)
    {
        GameObjectToRelayTo.TriggerParticleCollision(other);
    }
}
