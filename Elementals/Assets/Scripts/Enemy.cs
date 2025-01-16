using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : NetworkBehaviour
{
    private ObjectPool<Enemy> _pool;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("Enemy Spawned");
    }

    public void SetPool(ObjectPool<Enemy> pool)
    {
        _pool = pool;
    }

    public void ResetState()
    {
        
    }
}

