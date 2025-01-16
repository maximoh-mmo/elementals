using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawnFactory : MonoBehaviour
{
    [SerializeField] private int maxEnemies = 1000;
    ObjectPool<Enemy> _pool;
    [SerializeField]
    private Enemy _enemy;
    
    private void Awake()
    {
        _pool = new ObjectPool<Enemy>(CreateEnemy, OnTakeEnemyFromPool, OnReturnEnemyToPool, OnDestroyEnemy,true, maxEnemies,maxEnemies);
    }

    private void OnReturnEnemyToPool(Enemy enemy)
    {
        enemy.GetComponent<NetworkObject>().Despawn(false);
        enemy.ResetState(); // Custom method in Enemy to reset variables, if necessary
    }

    private void OnTakeEnemyFromPool(Enemy enemy)
    {
        enemy.GetComponent<NetworkObject>().Spawn();
    }

    private void OnDestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    private Enemy CreateEnemy()
    {
        Enemy enemy = Instantiate(_enemy, transform.position, Quaternion.identity);
        enemy.SetPool(_pool);
        return enemy;
    }

    public Enemy SpawnEnemy(Vector3 position, Quaternion rotation = default)
    {
        if (_pool.CountActive < maxEnemies)
        {
            var enemy = _pool.Get();
            enemy.transform.position = position;
            enemy.transform.rotation = rotation == default ? Quaternion.identity : rotation;
            return enemy;
        }
        else
        {
            Debug.Log("Max enemies reached");
        }
        return null;
    }
    
}