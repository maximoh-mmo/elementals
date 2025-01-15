using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawnFactory : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private int maxEnemies = 100;
    ObjectPool<Enemy> _pool;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 2f, 6f);
    }

    void SpawnEnemy()
    {
        Enemy instance = Instantiate(_enemy);
        instance.GetComponent<NetworkObject>().Spawn();
    }
}