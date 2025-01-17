using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : NetworkBehaviour
{
    private ObjectPool<Enemy> _pool;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("Enemy Spawned");
        
        if (!_navMeshAgent)
        {
            UnityEngine.AI.NavMeshHit closestHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out closestHit, 500, 1))
            {
                transform.position = closestHit.position;
                _navMeshAgent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
                gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
        }
    }

    public void SetPool(ObjectPool<Enemy> pool)
    {
        _pool = pool;
    }

    public void ResetState()
    {
        
    }
}

