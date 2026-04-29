using UnityEngine;
using Unity.Netcode;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance { get; private set; }
    
    public bool SetBlackWhite()
    {
        return Random.value > 0.5;
    }
}
