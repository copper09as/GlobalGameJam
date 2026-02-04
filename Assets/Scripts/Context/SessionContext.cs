using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

public class SessionContext : GameContext
{
    public MaskCollection maskCollection;
    public string PlayeId;
    public string opponentId;
    public string RoomId;
    public List<RoomInfo> AvailableRooms = new ();
    private NetSystem netSystem;
    

    protected override void OnInitialize()
    {
        maskCollection = Resources.Load<MaskCollection>("MaskCollection");
        netSystem = GameEntry.Instance.GetSystem<NetSystem>();
        netSystem.AddMsgListener("MsgShowRoomList", UpdateRoomList);
    }

    private void UpdateRoomList(MsgBase msgBase)
    {
        AvailableRooms = ((MsgShowRoomList)msgBase).RoomList;
    }

    protected override void OnDispose()
    {
        netSystem.RemoveListener("MsgShowRoomList", UpdateRoomList);
    }
}
