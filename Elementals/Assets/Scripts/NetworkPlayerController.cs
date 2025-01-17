using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


public class NetworkPlayerController : NetworkBehaviour
{
    // network variables
    public NetworkVariable<Vector3> position = new();
    public NetworkVariable<Quaternion> rotation = new();
    
    // player control settings
    [Header("Character Settings")]
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 200f; // Speed of rotation
    [SerializeField] private float attackRange = 5f;
    
    private NavMeshAgent _navMeshAgent;
    private Camera _mainCamera;
    private InputSystem_Actions _inputSystem;
    private InputAction _move;
    private Vector2 _lookInput;
    private bool _isRightMouseHeld = false;
    private bool _navMeshAgentMoving = false;
    
    [Rpc(SendTo.Server)]
    void PositionUpdateServerRpc(Vector3 position, RpcParams rpcParams = default) => this.position.Value = position;
    [Rpc(SendTo.Server)]
    void RotationUpdateServerRpc(Quaternion rotation, RpcParams rpcParams = default) => this.rotation.Value = rotation;

    private void OnEnable()
    {
        _inputSystem.Enable();
        // Subscribe to the input events
        if (!_navMeshAgent)
        {
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(transform.position, out closestHit, 500, 1))
            {
                transform.position = closestHit.position;
                _navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            }
        }
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

    private void OnPositionChanged(Vector3 previousvalue, Vector3 newvalue)
    {
        if (!IsOwner)
        {
            transform.position = position.Value;
        }
    }

    private void OnRotationChanged(Quaternion previousvalue, Quaternion newvalue)
    {
        if (!IsOwner)
        {
            transform.rotation = rotation.Value;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        RotationUpdateServerRpc(transform.rotation);
        PositionUpdateServerRpc(transform.position);
    }

    private void Awake()
    {
        // Initialization and References
        _inputSystem = new InputSystem_Actions();
        _mainCamera = GetComponentInChildren<Camera>();
        
        // Server RPCs 
        position.OnValueChanged += OnPositionChanged;
        rotation.OnValueChanged += OnRotationChanged;
    }
    private void RotateCharacter()
    {
        float yaw = _lookInput.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, yaw, 0);
        RotationUpdateServerRpc(transform.rotation);
    }

    private void MoveCharacter(Vector2 moveDirection)
    {
        Vector3 movement = transform.TransformDirection(new Vector3(moveDirection.x, 0, moveDirection.y) * (movementSpeed * Time.deltaTime));
        transform.Translate(movement, Space.World);
        PositionUpdateServerRpc(transform.position);
    }
    private bool IsEnemy(RaycastHit hit)
    {
        return hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy");
    }
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
    private void HandleClick()
    {
        // Raycast from the mouse position to the world
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the object is an enemy
            if (IsEnemy(hit))
            {
                AttackEnemy(hit.collider.gameObject);
            }
            else
            {
                // Otherwise, move to the clicked point
                _navMeshAgent.SetDestination(hit.point);
            }
        }
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

}