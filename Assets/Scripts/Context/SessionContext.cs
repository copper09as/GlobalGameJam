using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

public class SessionContext : GameContext
{
    public MaskCollection maskCollection;
    public string PlayerName;
    public string RoomId;
    public List<RoomInfo> AvailableRooms = new ();

    protected override void OnInitialize()
    {
        maskCollection = Resources.Load<MaskCollection>("MaskCollection");
    }
    protected override void OnDispose()
    {
    }
}
