using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

public class SessionContext : GameContext
{
    public MaskCollection maskCollection;
    public GunCollection gunCollection;
    public string PlayeId;
    public string opponentId;
    public string RoomId;
    public Gun localGun;
    public List<RoomInfo> AvailableRooms = new ();
    public PlayerData LocalPlayerData = new PlayerData(); // 玩家数据
    private NetSystem netSystem;
    
    
    protected override void OnInitialize()
    {
        maskCollection = Resources.Load<MaskCollection>("MaskCollection");
        gunCollection = Resources.Load<GunCollection>("Gun/GunCollection");
        netSystem = GameEntry.Instance.GetSystem<NetSystem>();
        netSystem.AddMsgListener("MsgShowRoomList", UpdateRoomList);
        localGun = gunCollection.GetGunById(0);

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
[Serializable]
public class PlayerData
{
    public int Gold;
    public List<int> OwnedGun = new();
}