using UnityEngine;
using UnityEngine.InputSystem;

public class ClickBoard : MonoBehaviour
{
    [Header("바둑판")]
    [SerializeField] private GameObject _badukpan;
    
    [Header("바둑알")]
    [SerializeField] private GameObject _Black;
    [SerializeField] private GameObject _White;
    
    [Header("카메라와 바둑판 사이 거리")]
    [SerializeField] private float _distance;
    private float _distanceMax;

    // 흑돌 차례일때 true, 백돌 차례일때 false
    private bool _isFirst;

    // 바둑판 레이어 마스크 저장
    private int _badukpanLayer;

    private void Awake()
    {
        _isFirst = true;
        _badukpanLayer = 1 << LayerMask.NameToLayer("Board");
        _distanceMax = _distance + 20f;
    }
    
    // 뉴 인풋 시스템의 클릭 이용
    public void OnClickBoard()
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
            screenPos.y = _badukpan.transform.position.y + 0.1f;
            SetBadukal(screenPos);
            // 흑 백 전환
            _isFirst = !_isFirst;
        }
    }

    // 레이를 쐇을때의 오브젝트 레이어 마스크가 Board이면 true, 나머지는 false
    private bool CheckBadukPan()
    {
        Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());

        if (Physics.Raycast(ray, _distanceMax, _badukpanLayer))
        {
            return true;
        }
        
        return false;
    }

    // 바둑알 놓기
    private void SetBadukal(Vector3 screenPos)
    {
        GameObject nowSetStone;
        if (_isFirst)
        {
            nowSetStone = _Black;
        }
        else
        {
            nowSetStone = _White;
        }
        
        // Todo: 나중에 오브젝트 풀링으로 로직 재구현 예정
        Instantiate(nowSetStone, screenPos, Quaternion.identity);
    }
}
