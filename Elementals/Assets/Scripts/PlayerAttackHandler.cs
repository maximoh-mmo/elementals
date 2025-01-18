using Unity.Netcode;
using UnityEngine;


public class NetworkPlayerController : NetworkBehaviour
{
    
    
    private void AttackEnemy(GameObject enemy)
    {
        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        if (distance <= attackRange)
        {
            Debug.Log($"Attacking {enemy.name}!");
        }
        else
        {
            Debug.Log($"{enemy.name} is out of range!");
        }
    }
   
    

}