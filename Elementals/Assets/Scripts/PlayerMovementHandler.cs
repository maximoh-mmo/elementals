using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovementHandler : NetworkBehaviour
{
    // network variables
    public NetworkVariable<Vector3> _position = new();
    public NetworkVariable<Quaternion> _rotation = new();
    public NetworkVariable<bool> _isWalking = new();
    
    private NavMeshAgent _navMeshAgent;
    private Player2 _player;
    
    public NavMeshAgent NavMeshAgent { set => _navMeshAgent = value; }

    [Rpc(SendTo.Server)]
    void PositionUpdateServerRpc(Vector3 position, RpcParams rpcParams = default)
        => _position.Value = position;
    [Rpc(SendTo.Server)]
    void RotationUpdateServerRpc(Quaternion rotation, RpcParams rpcParams = default)
        => _rotation.Value = rotation;
    
    [Rpc(SendTo.Server)]
    void MoveStatusUpdateServerRpc(bool isWalking, RpcParams rpcParams = default)
        => _isWalking.Value = isWalking;

    private void Start()
    {
        _player = GetComponent<Player2>();
        if (_navMeshAgent!=null)
            _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        _position.OnValueChanged += OnPositionChanged;
        _rotation.OnValueChanged += OnRotationChanged;
        _isWalking.OnValueChanged += OnWalkingChanged;
    }

    private void Update()
    {
        if (IsOwner && !IsServer)
        {
            if (_navMeshAgent.velocity.sqrMagnitude > 0 && !_isWalking.Value)
            {
                MoveStatusUpdateServerRpc(true);
            }
            if (_navMeshAgent.velocity.sqrMagnitude == 0 && _isWalking.Value)
            {
                MoveStatusUpdateServerRpc(false);
            }
            if (_isWalking.Value)
            {
                PositionUpdateServerRpc(transform.position);
                RotationUpdateServerRpc(transform.rotation);
            }
        }
        if (IsServer)
        {
            transform.position = _position.Value;
            transform.rotation = _rotation.Value;
        }
    }

    public void RotateCharacter(Vector2 lookInput)
    {
        float yaw = lookInput.x * _player.RotationSpeed * Time.deltaTime;
        transform.Rotate(0, yaw, 0);
        var newRotation = transform.rotation;
        RotationUpdateServerRpc(newRotation);
    }

    public void MoveCharacter(Vector2 moveDirection)
    {
        var forward = transform.forward;
        var right = transform.right;
        var movement = ((moveDirection.y * forward) + (moveDirection.x * right)) * (_player.MovementSpeed * Time.deltaTime);
        transform.position += movement;
        PositionUpdateServerRpc(transform.position);
    }
    private void OnPositionChanged(Vector3 previousvalue, Vector3 newvalue)
    {
        if (!IsOwner)
            transform.position = _position.Value;
    }

    private void OnRotationChanged(Quaternion previousvalue, Quaternion newvalue)
    {
        if (!IsOwner)
            transform.rotation = _rotation.Value;
    }

    private void OnWalkingChanged(bool oldvalue, bool newvalue)
    {
        var animator = GetComponent<Animator>();
        if (animator!=null)
        {
            animator.SetBool("moving", newvalue);
        }
    }

    public void NavigateTo(Vector3 position)
    {
        if (_navMeshAgent == null)
            return;
        _navMeshAgent.SetDestination(position);
    }
}