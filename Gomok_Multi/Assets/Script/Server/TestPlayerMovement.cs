using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    
    private InputSystem_Actions _playerInput;
    private Vector2 _inputDirection;
    private NetworkVariable<Vector2> _moveVariable;

    private void Awake()
    {
        _playerInput = new InputSystem_Actions();
        _moveVariable = new NetworkVariable<Vector2>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            _playerInput.Player.Disable();
            return;
        }
        
        _playerInput.Player.Enable();
    }

    private void OnEnable()
    {
        _playerInput.Player.Move.performed += Moving;
        _playerInput.Player.Move.canceled += Moving;
    }

    private void OnDisable()
    {
        _playerInput.Player.Move.performed -= Moving;
        _playerInput.Player.Move.canceled -= Moving;
        _playerInput.Player.Disable();
    }

    [ServerRpc]
    private void RequestMoveServerRpc(Vector2 move)
    {
        _moveVariable.Value = move;
    }
    
    private void Moving(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        
        var tempVector2 = context.ReadValue<Vector2>();
        RequestMoveServerRpc(tempVector2);
    }

    public void FixedUpdate()
    {
        if (_moveVariable.Value != Vector2.zero)
        {
            Vector3 move = _moveSpeed * Time.deltaTime * new Vector3(_moveVariable.Value.x, 0f, _moveVariable.Value.y);
            transform.position += move;
        }
    }
}
