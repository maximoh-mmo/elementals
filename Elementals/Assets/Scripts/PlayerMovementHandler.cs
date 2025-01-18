using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovementHandler : NetworkBehaviour
{
    // network variables
    public NetworkVariable<Vector3> position = new();
    public NetworkVariable<Quaternion> rotation = new();
    
    private Camera _mainCamera;
    private InputSystem_Actions _inputSystem;
    private InputAction _move;
    private bool _isRightMouseHeld = false;
    private bool _navMeshAgentMoving = false;
    private NavMeshAgent _navMeshAgent;
    private Vector3 _lookInput;
    private Player _player;
    
    [Rpc(SendTo.Server)]
    void PositionUpdateServerRpc(Vector3 position, RpcParams rpcParams = default) => this.position.Value = position;
    [Rpc(SendTo.Server)]
    void RotationUpdateServerRpc(Quaternion rotation, RpcParams rpcParams = default) => this.rotation.Value = rotation;
    private void Awake()
    {
        _mainCamera = Camera.main;
        _player = GetComponentInParent<Player>();
    }

    private void OnEnable()
    {
        _inputSystem.Enable();
        // Subscribe to the input events
        
        _move = _inputSystem.Player.Move;
        _inputSystem.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _inputSystem.Player.Look.canceled += ctx => _lookInput = Vector2.zero;
        _inputSystem.Player.RMB.performed += ctx => _isRightMouseHeld = true;
        _inputSystem.Player.RMB.canceled += ctx => _isRightMouseHeld = false;
        _inputSystem.Player.LMB.performed += ctx => HandleClick();
        _move.Enable();
    }
    private void OnDisable()
    {
        position.OnValueChanged -= OnPositionChanged;
        rotation.OnValueChanged -= OnRotationChanged;
        _inputSystem.Disable();
    }
    void Update(){
        if (IsOwner && !IsServer)
        {
            _navMeshAgentMoving = _navMeshAgent.velocity.sqrMagnitude > 0;
            if (_navMeshAgentMoving)
            {
                PositionUpdateServerRpc(transform.position);
                RotationUpdateServerRpc(transform.rotation);
            }
            var moveDirection = _move.ReadValue<Vector2>();
            if (_isRightMouseHeld)
            {
                RotateCharacter();
            }
            if (moveDirection != Vector2.zero)
            {
                MoveCharacter(moveDirection);
            }
        }
        if (IsServer)
        {
            transform.position = position.Value;
            transform.rotation = rotation.Value;
        }
    }
    private bool IsEnemy(RaycastHit hit)
    {
        return hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy");
    }
    private void HandleClick()
    {
        // Raycast from the mouse position to the world
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (!IsEnemy(hit))
            {
                _navMeshAgent.SetDestination(hit.point);
            }
        }
    }
    private void RotateCharacter()
    {
        float yaw = _lookInput.x * _player.RotationSpeed * Time.deltaTime;
        transform.Rotate(0, yaw, 0);
        RotationUpdateServerRpc(transform.rotation);
    }

    private void MoveCharacter(Vector2 moveDirection)
    {
        Vector3 movement = transform.TransformDirection(new Vector3(moveDirection.x, 0, moveDirection.y) * (_player.MovementSpeed * Time.deltaTime));
        transform.Translate(movement, Space.Self);
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

    public void SetNavMeshAgent(NavMeshAgent navMeshAgent)
    {
        if (navMeshAgent != null)
            _navMeshAgent = navMeshAgent;
    }
}