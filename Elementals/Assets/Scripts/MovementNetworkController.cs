using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class MovementNetworkController : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new();
    
    private float movementSpeed = 5.0f;

    private InputSystem_Actions _inputSystem;
    private InputAction _move;
    private InputAction _fire;


    [Rpc(SendTo.Server)]
    void PositionUpdateServerRpc(Vector3 position, RpcParams rpcParams = default) => Position.Value = position;

    private void OnEnable()
    {
        _move = _inputSystem.Player.Move;
        _move.Enable();
    }

    private void OnDisable()
    {
        Position.OnValueChanged -= OnPositionChanged;
        _move.Disable();
    }

    private void OnPositionChanged(Vector3 previousvalue, Vector3 newvalue)
    {
        if (!IsOwner)
        {
            transform.position = Position.Value;
        }
    }

    private void Awake()
    {
        _inputSystem = new InputSystem_Actions();
        Position.OnValueChanged += OnPositionChanged;
    }

    void Update(){
        if (IsOwner && !IsServer)
        {
            var moveDirection = _move.ReadValue<Vector2>();
            Vector3 movement = new Vector3(moveDirection.x, 0, moveDirection.y) * (movementSpeed * Time.deltaTime);
            transform.Translate(movement, Space.World);
            PositionUpdateServerRpc(transform.position);
        }
        if (IsServer)
        {
            transform.position = Position.Value;
        }
    }

}