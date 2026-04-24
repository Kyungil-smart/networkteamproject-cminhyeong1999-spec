using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Badukpan : MonoBehaviour
{
    [Header("바둑판 맨 왼쪽 아래 위치")]
    [SerializeField] private GameObject _badukPanBottomLeftPoint;
    [Header("바둑판 맨 오른쪽 위 위치")]
    [SerializeField] private GameObject _badukPanTopRightPoint;
    
    [Header("바둑알")]
    [SerializeField] private GameObject _Black;
    [SerializeField] private GameObject _White;
    
    public Vector3[,] _badukpanPositionArray { get; private set; }

    private const int _BadukpanSize = 19;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _badukpanPositionArray = new Vector3[_BadukpanSize, _BadukpanSize];

        // 19 X 19 바둑판의 줄 간격개수는 18개
        // 바둑판 간격 계산시에는 19 - 1 을 하여 계산
        float badukpanSizeIntToFloat = (float)(_BadukpanSize - 1);
        
        for (int y = 0; y < _BadukpanSize; y++)
        {
            for (int x = 0; x < _BadukpanSize; x++)
            {
                // 현재 칸에서 다음 칸 까지 얼마나 떨어져 있는지를 나타내는 수치
                float badukPanXGap = x / badukpanSizeIntToFloat;
                float badukPanZGap = y / badukpanSizeIntToFloat;
                
                float xPos = Mathf.Lerp(_badukPanBottomLeftPoint.transform.position.x, _badukPanTopRightPoint.transform.position.x, badukPanXGap);
                float zPos = Mathf.Lerp(_badukPanBottomLeftPoint.transform.position.z, _badukPanTopRightPoint.transform.position.z, badukPanZGap);
            
                _badukpanPositionArray[x, y] = new Vector3(xPos, 0, zPos);
            }
        }
    }

    // Player Input에 임시로 Test 액션을 등록해둠
    void OnTest()
    {
        PrintPosition();
    }

    // 각 포지션을 순회하며 큐브를 두는 메서드
    // 격자 위에 돌이 잘 위치하는지를 보기위한 테스트코드
    private void PrintPosition()
    {
        foreach (var item in _badukpanPositionArray)
        {
            Instantiate(_Black, item, Quaternion.identity);
        }
    }
}
