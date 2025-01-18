using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovementHandler : NetworkBehaviour
{
    // network variables
    public NetworkVariable<Vector3> position = new();
    public NetworkVariable<Quaternion> rotation = new();
    public NetworkVariable<bool> isWalking = new();
    
    private NavMeshAgent _navMeshAgent;
    private Player _player;
    
    public NavMeshAgent NavMeshAgent { set => _navMeshAgent = value; }

    [Rpc(SendTo.Server)]
    void PositionUpdateServerRpc(Vector3 position, RpcParams rpcParams = default) => this.position.Value = position;
    [Rpc(SendTo.Server)]
    void RotationUpdateServerRpc(Quaternion rotation, RpcParams rpcParams = default) => this.rotation.Value = rotation;

    private void Start()
    {
        _player = GetComponent<Player>();
        if (_navMeshAgent!=null)
            _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (IsOwner && !IsServer)
        {
            isWalking.Value = _navMeshAgent.velocity.sqrMagnitude > 0;
            Debug.Log(_navMeshAgent.velocity.sqrMagnitude);
            if (isWalking.Value)
            {
                PositionUpdateServerRpc(transform.position);
                RotationUpdateServerRpc(transform.rotation);
            }
        }

        if (IsServer)
        {
            transform.position = position.Value;
            transform.rotation = rotation.Value;
        }
    }

    public void RotateCharacter(Vector2 lookInput)
    {
        float yaw = lookInput.x * _player.RotationSpeed * Time.deltaTime;
        transform.Rotate(0, yaw, 0);
        RotationUpdateServerRpc(transform.rotation);
    }

    public void MoveCharacter(Vector2 moveDirection)
    {
        Vector3 movement = transform.TransformDirection(new Vector3(moveDirection.x, 0, moveDirection.y) * (_player.MovementSpeed * Time.deltaTime));
        transform.Translate(movement, Space.World);
        PositionUpdateServerRpc(transform.position);
    }
    private void OnPositionChanged(Vector3 previousvalue, Vector3 newvalue)
    {
        if (!IsOwner)
            transform.position = position.Value;
    }

    private void OnRotationChanged(Quaternion previousvalue, Quaternion newvalue)
    {
        if (!IsOwner)
            transform.rotation = rotation.Value;
    }

    public void NavigateTo(Vector3 position)
    {
        _navMeshAgent.SetDestination(position);
    }
}