using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    
    private Vector2 _inputDirection;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        // 소유자 전용 입력 바인딩 등 초기화
    }

    public void OnMove(InputValue value)
    {
        if (!IsOwner) return;
        
        _inputDirection = value.Get<Vector2>();
    }

    public void FixedUpdate()
    {
        if (!IsOwner) return;
        
        if (_inputDirection != Vector2.zero)
        {
            Vector3 move = new Vector3(_inputDirection.x, 0f, _inputDirection.y) * _moveSpeed * Time.deltaTime;
            transform.position += move;
        }
    }
}
