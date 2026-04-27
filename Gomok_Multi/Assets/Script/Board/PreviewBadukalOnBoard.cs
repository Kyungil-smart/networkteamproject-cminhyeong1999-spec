using UnityEngine;
using UnityEngine.InputSystem;

public class PreviewBadukalOnBoard : MonoBehaviour
{
    [Header("바둑판")]
    [SerializeField] private Badukpan _badukpan;
    
    [Header("바둑알 섀도우")]
    [SerializeField] private GameObject _blackShadow;
    [SerializeField] private GameObject _whiteShadow;
    
    [Header("카메라와 바둑판 사이 거리")]
    [SerializeField] private float _distance;
    private float _distanceMax;

    // 흑돌을 표시해야하면 true, 아니면 false
    private bool _isBlack;
    private bool _isActive;
    
    // 바둑판 레이어 마스크 저장
    private int _badukpanLayer;

    private void Awake()
    {
        // Todo: 현재 플레이어가 흑돌인지 백돌인지 정보를 받아오는 작업 진행 예정
        _isBlack = true;
        _isActive = true;
        _badukpanLayer = 1 << LayerMask.NameToLayer("Board");
        _distanceMax = _distance + 20f;
        
        _blackShadow = Instantiate(_blackShadow);
        _whiteShadow = Instantiate(_whiteShadow);
        
        _blackShadow.SetActive(false);
        _whiteShadow.SetActive(false);
    }
    
    private void Update()
    {
        ShotRay();
    }

    private void ShotRay()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
        Vector3 screenPos = Vector3.zero;
        
        if (Physics.Raycast(ray, out hit, _distanceMax, _badukpanLayer))
        {
            screenPos = hit.point;
            screenPos.y = _badukpan.transform.position.y + 0.1f;
            _isActive = true;
            ShowBadukalShadow(screenPos);
        }
        else
        {
            _isActive = false;
            screenPos = Vector3.zero;
            ShowBadukalShadow(screenPos);
        }
    }

    private void ShowBadukalShadow(Vector3 screenPos)
    {
        GameObject nowShowStone;
        if(_isBlack) 
            nowShowStone = _blackShadow;
        else 
            nowShowStone = _whiteShadow;
        
        nowShowStone.SetActive(_isActive);
        
        Vector3 tempPos = _badukpan.CheckBadukpanPosition(screenPos,_badukpan.BadukalColor);
        nowShowStone.transform.position = tempPos;
    }
    
}
