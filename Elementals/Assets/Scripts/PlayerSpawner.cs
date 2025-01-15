using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Player _player;

    void SpawnPlayer()
    {
        Player instance = Instantiate(_player);
        instance.GetComponent<NetworkObject>().Spawn();
    }
}