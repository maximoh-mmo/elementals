using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private int maxEnemies;
    private EnemySpawnFactory factory;
    [SerializeField]
    private float SpawnRadius;
    List<Enemy> enemies;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        factory = FindFirstObjectByType<EnemySpawnFactory>();
        enemies = new List<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        while(enemies.Count < maxEnemies)
        {
            enemies.Add(factory.SpawnEnemy(EnemySpawnPosition()));
        }
    }

    private Vector3 EnemySpawnPosition()
    {
        Vector3 position = Vector3.zero;
        Vector3 center = transform.position;
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Terrain", "Water");
        do
        {
            position.x = Random.Range(-SpawnRadius, SpawnRadius);
            position.z = Random.Range(-SpawnRadius, SpawnRadius);
            position += center;
            if (Physics.Raycast(new Vector3(position.x, 9999f, position.z), Vector3.down, out hit, Mathf.Infinity,
                    mask))
            {
                position.y = hit.point.y;
            }
            else
            {
                position.y = 0;
            }
        } while (position.y == 0);
        
        return position;
    }
}
