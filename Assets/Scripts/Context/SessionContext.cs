using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

public class SessionContext : GameContext
{
    public List<Wall> Walls = new ();
    public MaskCollection maskCollection;
    public string PlayerName;
    public Player SyncPlayer;
    public Player LocalPlayer;
    internal int playerType;

    protected override void OnInitialize()
    {
        maskCollection = Resources.Load<MaskCollection>("MaskCollection");
    }
    protected override void OnDispose()
    {
        // 会话清理逻辑
        Debug.Log("[SessionContext] 会话上下文清理");
    }
}
