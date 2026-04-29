using UnityEngine;
using Unity.Netcode;

public partial class Badukpan
{
    /// <summary>
    /// 클라이언트가 서버에게 돌을 둘 수 있는지를 요청하는 RPC
    /// </summary>
    /// <param name="pos">돌을 놓은 위치</param>
    /// <param name="stoneColor">현재 플레이어의 돌 색깔</param>
    [ServerRpc]
    public void PlaceBadukalServerRpc(Vector3 pos, StoneColor stoneColor)
    {
        var tempPos = SetBadukpanPosition(pos, stoneColor);

        if (tempPos == CanNotPlacedStone)
        {

            return;
        }
        
        GameObject setBadukal;

        if (stoneColor == StoneColor.Black)
        {
            setBadukal = _black;
        }
        else
        {
            setBadukal = _white;
        }
        
        Instantiate(setBadukal, tempPos, Quaternion.identity);
        
        CheckWin(tempPos);
    }
}
