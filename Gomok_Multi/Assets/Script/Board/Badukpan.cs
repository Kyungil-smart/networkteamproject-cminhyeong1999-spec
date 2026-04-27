using UnityEngine;

public class Badukpan : MonoBehaviour
{
    [Header("바둑판 맨 왼쪽 아래 위치")]
    [SerializeField] private GameObject _badukPanBottomLeftPoint;
    [Header("바둑판 맨 오른쪽 위 위치")]
    [SerializeField] private GameObject _badukPanTopRightPoint;
    
    [Header("현재 바둑알 색깔")]
    public StoneColor BadukalColor;
    
    public Vector3[,] _badukpanPositionArray { get; private set; }
    public float BadukpanDistance { get; private set; }
    // 이미 놓여진 곳에 착수 시도시 반환할 위치
    public Vector3 CanNotPlacedStone{ get; private set; }

    private const int _BadukpanSize = 19;
    private CheckFiveStone _checkFiveStone;
    
    private void Awake()
    {
        _badukpanPositionArray = new Vector3[_BadukpanSize, _BadukpanSize];
        _checkFiveStone = new CheckFiveStone();
        _checkFiveStone.Init();
        BadukalColor = StoneColor.Black;
        CanNotPlacedStone = new Vector3(-10f, -10f, -10f);

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

        // 격자간 거리 계산 메써드
        CalculateAverageDistance();
    }
    
    private void CalculateAverageDistance()
    {
        // 19x19 바둑판 기준, 인접한 두 칸의 거리 차이를 계산
        // [0, 0]과 [1, 0]의 차이는 X 간격
        float distX = Vector3.Distance(_badukpanPositionArray[0, 0], _badukpanPositionArray[1, 0]);
    
        // [0, 0]과 [0, 1]의 차이는 Z 간격
        float distZ = Vector3.Distance(_badukpanPositionArray[0, 0], _badukpanPositionArray[0, 1]);

        float averageDist = (distX + distZ) / 2f;
        
        BadukpanDistance = averageDist;
    }

    // 바둑알 프리뷰에 사용하는 메서드
    public Vector3 CheckBadukpanPosition(Vector3 screenPos, StoneColor stoneColor)
    {
        // 0,0을 기준으로
        Vector3 Center = _badukpanPositionArray[0, 0];
        
        // 기준에서부터 screenPos의 x,z를 계산
        // ex)  screenPos.x == 19.95, screenPos.z == 8.474, BadukpanDistance == 1.105583
        //      (19.95 - 0.03) / 1.105583 >> 18.0176 ... > x == 18
        //      (8.474 - 0.06) / 1.105583 >> 7.6104 ... > z == 8
        //          >>>>    _badukpanPositionArray[18, 8] > 맨 왼쪽 아래점을 (0,0)으로 18,8에 위치한 점
        int x = Mathf.RoundToInt((screenPos.x - Center.x) / BadukpanDistance);
        int z = Mathf.RoundToInt((screenPos.z - Center.z) / BadukpanDistance);
        
        int width = _badukpanPositionArray.GetLength(0);
        int height = _badukpanPositionArray.GetLength(1);
        
        // 최댓값 이상을 넘지 않도록 방어
        x = Mathf.Clamp(x, 0, width - 1);
        z = Mathf.Clamp(z, 0, height - 1);
        
        return _badukpanPositionArray[x, z];
    }
    
    // 실제로 바둑알을 놓을 시 사용하는 메서드
    public Vector3 SetBadukpanPosition(Vector3 screenPos, StoneColor stoneColor)
    {
        // 0,0을 기준으로
        Vector3 Center = _badukpanPositionArray[0, 0];
        
        // 기준에서부터 screenPos의 x,z를 계산
        // ex)  screenPos.x == 19.95, screenPos.z == 8.474, BadukpanDistance == 1.105583
        //      (19.95 - 0.03) / 1.105583 >> 18.0176 ... > x == 18
        //      (8.474 - 0.06) / 1.105583 >> 7.6104 ... > z == 8
        //          >>>>    _badukpanPositionArray[18, 8] > 맨 왼쪽 아래점을 (0,0)으로 18,8에 위치한 점
        int x = Mathf.RoundToInt((screenPos.x - Center.x) / BadukpanDistance);
        int z = Mathf.RoundToInt((screenPos.z - Center.z) / BadukpanDistance);
        
        int width = _badukpanPositionArray.GetLength(0);
        int height = _badukpanPositionArray.GetLength(1);
        
        // 최댓값 이상을 넘지 않도록 방어
        x = Mathf.Clamp(x, 0, width - 1);
        z = Mathf.Clamp(z, 0, height - 1);

        if (IsPlaced(x, z))
        {
            Debug.Log($"이미 돌이 놓여저 있습니다!");
            return CanNotPlacedStone;
        }

        SetIsPlaced(x, z, true, stoneColor);
        return _badukpanPositionArray[x, z];
    }

    public void CheckWin(Vector3 screenPos)
    {
        // 0,0을 기준으로
        Vector3 Center = _badukpanPositionArray[0, 0];
        
        // 기준에서부터 screenPos의 x,z를 계산
        // ex)  screenPos.x == 19.95, screenPos.z == 8.474, BadukpanDistance == 1.105583
        //      (19.95 - 0.03) / 1.105583 >> 18.0176 ... > x == 18
        //      (8.474 - 0.06) / 1.105583 >> 7.6104 ... > z == 8
        //          >>>>    _badukpanPositionArray[18, 8] > 맨 왼쪽 아래점을 (0,0)으로 18,8에 위치한 점
        int x = Mathf.RoundToInt((screenPos.x - Center.x) / BadukpanDistance);
        int z = Mathf.RoundToInt((screenPos.z - Center.z) / BadukpanDistance);
        
        int width = _badukpanPositionArray.GetLength(0);
        int height = _badukpanPositionArray.GetLength(1);
        
        // 최댓값 이상을 넘지 않도록 방어
        x = Mathf.Clamp(x, 0, width - 1);
        z = Mathf.Clamp(z, 0, height - 1);

        if (_checkFiveStone.CheckWin(x, z, BadukalColor))
        {
            Debug.Log($"{BadukalColor} 승리");
        }
    }
    
    public bool IsPlaced(int x, int z) => _checkFiveStone.IsPlaced(x, z);
    public void SetIsPlaced(int x, int z, bool value, StoneColor stoneColor) => _checkFiveStone.SetIsPlaced(x, z, value, stoneColor);
    
    
}
