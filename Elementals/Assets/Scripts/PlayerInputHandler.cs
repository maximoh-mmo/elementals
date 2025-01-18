using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private InputSystem_Actions _inputSystem;
        private InputAction _move;
        private bool _isRightMouseHeld = false;
        private Vector2 _lookInput;
        private PlayerMovementHandler _playerMovementHandler;
        private PlayerAttackHandler _playerAttackHandler;
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = FindFirstObjectByType<Camera>();
            if (!_playerMovementHandler)
                _playerMovementHandler = GetComponent<PlayerMovementHandler>();
        }

        private void OnEnable()
        {
            _inputSystem = new InputSystem_Actions();
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

        void Update()
        {
            
            var moveDirection = _move.ReadValue<Vector2>();
            if (moveDirection.x != 0 || moveDirection.y != 0)
            {
                Debug.Log(moveDirection);
            }

            if (_isRightMouseHeld)
            {
                _playerMovementHandler.RotateCharacter(_lookInput);
            }

            if (moveDirection != Vector2.zero)
            {
                _playerMovementHandler.MoveCharacter(moveDirection);
            }
        }

        private void OnDisable()
        {
            _inputSystem.Disable();
        }
        
        private void HandleClick()
        {
            // Raycast from the mouse position to the world
            if (_mainCamera == null)
            {
                Debug.LogWarning("Main camera not set");
                return;
            }
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!IsEnemy(hit))
                {
                    _playerMovementHandler.NavigateTo(hit.point);
                }
            }
        }
        private bool IsEnemy(RaycastHit hit)
        {
            return hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy");
        }
    }
}