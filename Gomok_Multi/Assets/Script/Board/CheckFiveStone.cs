using System.Collections.Generic;

public class CheckFiveStone
{
    private const int BADUKPAN_SIZE = 19;
    private static readonly (int x, int z)[] DIRECTIONS = {
        (1, 0),  // 가로
        (0, 1),  // 세로
        (1, 1),  // 왼쪽 위 대각선
        (1, -1)  // 왼쪽 아래 대각선
    };
    
    private bool[,] _isPlaced = new bool[BADUKPAN_SIZE, BADUKPAN_SIZE];
    
    // 흑돌, 백돌이 바둑판상 어느 위치에 있는지를 저장
    private HashSet<(int x, int z)> _isPlacedListBlack = new();
    private HashSet<(int x, int z)> _isPlacedListWhite = new();

    public void Init()
    {
        for (int i = 0; i < BADUKPAN_SIZE; i++)
        {
            for (int j = 0; j < BADUKPAN_SIZE; j++)
            {
                _isPlaced[i, j] = false;
            }
        }
        
        _isPlacedListBlack.Clear();
        _isPlacedListWhite.Clear();
    }

    public bool IsPlaced(int x, int y)
    {
        return _isPlaced[x, y];
    }
    
    public void SetIsPlaced(int x, int z, bool value, StoneColor stoneColor)
    {
        if (stoneColor == StoneColor.Black)
        {
            _isPlacedListBlack.Add((x, z));
        }
        else
        {
            _isPlacedListWhite.Add((x, z));
        }
        
        _isPlaced[x, z] = value;
    }
    
    /// <summary>
    /// 승리 체크 메서드
    /// </summary>
    /// <param name="lastX">돌을 둔 격자의 x좌표</param>
    /// <param name="lastZ">돌을 둔 격자의 z좌표</param>
    /// <param name="stoneColor">현재 플레이어의 돌 색깔</param>
    /// <returns>승리 조건에 맞으면 true, 그외는 false</returns>
    public bool CheckWin(int lastX, int lastZ, StoneColor stoneColor)
    {
        return FreeStyleRule(lastX, lastZ, stoneColor);
    }

    /// <summary>
    /// 자유룰 / 3,3 이나 4,4 등등의 금지 구역이나 금지룰 없음
    /// </summary>
    /// <param name="lastX">돌을 둔 격자의 x좌표</param>
    /// <param name="lastZ">돌을 둔 격자의 z좌표</param>
    /// <param name="stoneColor">현재 플레이어의 돌 색깔</param>
    /// <returns>승리 조건에 맞으면 true, 그외는 false</returns>
    private bool FreeStyleRule(int lastX, int lastZ, StoneColor stoneColor)
    {
        HashSet<(int x, int z)> nowCheckStones;
        
        if (stoneColor == StoneColor.Black)
        {
            nowCheckStones = _isPlacedListBlack;
        }
        else
        {
            nowCheckStones = _isPlacedListWhite;
        }
        
        foreach (var dir in DIRECTIONS)
        {
            // 자기 자신을 기준으로 5개 이상의 돌이 연결되는것이 승리 조건임으로 1부터 셈
            int count = 1;

            // 한쪽 방향으로 탐색 후 그와 반대 방향으로 탐색
            // ex) 왼쪽 위 방향으로 탐색 >> [1,1]방향으로 탐색 후 [-1,-1]방향으로 탐색
            count += CheckStones(lastX, lastZ, dir.x, dir.z, nowCheckStones);
            count += CheckStones(lastX, lastZ, -dir.x, -dir.z, nowCheckStones);
            
            // 두 방향으로 탐색성공한 횟수가 5 이상이면 승리
            if (count >= 5) return true;
        }
        return false;
    }

    /// <summary>
    /// 특정 방향에 같은 색 돌이 얼마나 놓여있는지를 체크하는 메서드
    /// </summary>
    /// <param name="startX">탐색을 시작할 x좌표</param>
    /// <param name="startZ">탐색을 시작할 z좌표</param>
    /// <param name="dirX">x좌표 방향</param>
    /// <param name="dirZ">z좌표 방향</param>
    /// <param name="nowCheckStones">흑돌이나 백돌이 어느좌표에 착수하였는지가 저장된 HashSet</param>
    /// <returns></returns>
    private int CheckStones(int startX, int startZ, int dirX, int dirZ, HashSet<(int x, int z)> nowCheckStones)
    {
        int count = 0;
        int nextX = startX + dirX;
        int nextZ = startZ + dirZ;

        // 연속해서 같은 색의 돌이 있는지 확인
        while (nowCheckStones.Contains((nextX, nextZ)))
        {
            count++;
            nextX += dirX;
            nextZ += dirZ;
        }
        return count;
    }
}
