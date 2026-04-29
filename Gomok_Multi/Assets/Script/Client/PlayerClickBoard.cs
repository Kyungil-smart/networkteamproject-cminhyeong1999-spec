using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClickBoard : NetworkBehaviour
{
    [Header("바둑알")]
    [SerializeField] private GameObject _Black;
    [SerializeField] private GameObject _White;
    
    [Header("현재 플레이어 돌 색깔")]
    [SerializeField] private StoneColor _stoneColor;
    
    // 바둑판과 카메라 사이 거리, 레이를 쏠 때 최대 거리 설정
    private float _distance;
    
    // 바둑판 Y좌표, 바둑판 위에 돌을 올려놓을 경우에 필요한 Y값
    private float _badukpanY;

    // 흑돌 차례일때 true, 백돌 차례일때 false
    private bool _isBlack;

    // 바둑판 레이어 마스크 저장
    private int _badukpanLayer;
    
    // 인풋 시스템
    private InputSystem_Actions _playerInput;
    
    // 기타 이유로 놓을 수 없는 곳을 반환할 경우의 위치
    private Vector3 DoNotPlaceStone;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        Init();
    }

    private void Init()
    {
        _isBlack = true;
        _badukpanLayer = 1 << LayerMask.NameToLayer("Board");
        _distance = 40f;
        _playerInput = new InputSystem_Actions();
        _playerInput.Enable();
        _playerInput.BadukActionMap.ClickBoard.performed += ClickToBoard;
        _badukpanY = Badukpan.Instance.transform.position.y;
        DoNotPlaceStone = Badukpan.Instance.CanNotPlacedStone;
    }
    
    /// <summary>
    /// 화면에서 바둑판을 클릭을 한 후에 실제로 바둑알을 바둑판 위에 올려놓는 메서드
    /// </summary>
    /// <param name="ctx"></param>
    public void ClickToBoard(InputAction.CallbackContext ctx)
    {
        // 바둑판을 클릭했는지 체크
        if (!CheckBadukPan()) return;
        
        // 마우스, 스크린을 클릭한 포인트를 바둑판에 매칭
        Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _badukpanLayer))
        {
            // 레이를 쏜 지점에 돌 놓기
            var screenPos = hit.point;
            screenPos.y = _badukpanY + 0.1f;
            Badukpan.Instance.PlaceBadukalServerRpc(screenPos, _stoneColor);
        }
    }

    /// <summary>
    /// 레이를 쐇을때 맞은 오브젝트의 레이어 마스크가 Board이면 true, 나머지는 false
    /// </summary>
    /// <returns></returns>
    private bool CheckBadukPan()
    {
        Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());

        if (Physics.Raycast(ray, _distance, _badukpanLayer))
        {
            return true;
        }
        
        return false;
    }
}
