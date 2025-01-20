using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Player2 : NetworkBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private CameraController _cameraController;
    // player control settings
    [Header("Character Settings")] [SerializeField]
    private float movementSpeed = 5.0f;

    [SerializeField] private float rotationSpeed = 20f; // Speed of rotation
    [SerializeField] private float attackRange = 5f;
    public float MovementSpeed { get => movementSpeed; }
    public float RotationSpeed { get => rotationSpeed; }
    public float AttackRange { get => attackRange; }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("Player Spawned... Initializing....\nCurrent Position is : " + this.transform.position);
        if (!_navMeshAgent && IsOwner)
        {
            _navMeshAgent = this.GetComponent<NavMeshAgent>();
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(transform.position, out closestHit, 50, NavMesh.GetAreaFromName("WalkableArea")))
            {
                transform.position = closestHit.position;
                if (!_navMeshAgent)
                    _navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            }
            else
            {
                Debug.Log("Could not create Navmesh component, no navmesh within distance");
            }
        }

        if (!IsServer && IsOwner)
        {
            var mainCamera = new GameObject("MainCamera").AddComponent<Camera>();
            mainCamera.transform.position += Vector3.forward * -10;
            mainCamera.transform.LookAt(transform.position);
            _cameraController = mainCamera.gameObject.AddComponent<CameraController>();
            _cameraController.CameraTarget = transform;
        }
        if (_navMeshAgent!=null)
            GetComponent<PlayerMovementHandler>().NavMeshAgent = _navMeshAgent;
    }
}