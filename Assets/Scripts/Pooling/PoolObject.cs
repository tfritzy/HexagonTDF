using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public int ObjectType;

    private Pool Owner;

    private const float ForceReturnTime = 300f;
    private float birthTime;

    public void Setup(int objectType, Pool owner)
    {
        this.birthTime = Time.time;
        this.ObjectType = objectType;
        this.Owner = owner;
    }

    void Update()
    {
        if (Time.time > birthTime + ForceReturnTime)
        {
            ReturnToPool();
        }
    }

    public void ReturnToPool(float delay)
    {
        if (delay < .01f)
        {
            ReturnToPool();
        }
        else
        {
            Invoke("ReturnToPool", delay);
        }
    }

    public void ReturnToPool()
    {
        Owner.ReturnObject(this.gameObject, this.ObjectType);
    }
}