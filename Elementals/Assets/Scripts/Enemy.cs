using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("Enemy Spawned");
    }
}

