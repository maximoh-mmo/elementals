using Unity.Netcode;
using UnityEngine;


public class PlayerAttackHandler : NetworkBehaviour
{
    private Player _player;
    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void AttackEnemy(GameObject enemy)
    {
        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        if (distance <= _player.AttackRange)
        {
            Debug.Log($"Attacking {enemy.name}!");
        }
        else
        {
            Debug.Log($"{enemy.name} is out of range!");
        }
    }
}