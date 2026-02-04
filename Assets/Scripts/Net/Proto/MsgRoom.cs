using System;
using System.Collections.Generic;

public class MsgShowRoomList : MsgBase
{
    public MsgShowRoomList()
    {
        protoName = "MsgShowRoomList";
    }
    public List<RoomInfo> RoomList = new List<RoomInfo>();
}
public class MsgCreateRoom : MsgBase
{
    public MsgCreateRoom()
    {
        protoName = "MsgCreateRoom";
    }
    public string roomId = "";
    public int result = 0;
}
public class MsgJoinRoom : MsgBase
{
    public MsgJoinRoom()
    {
        protoName = "MsgJoinRoom";
    }
    public string roomId;
    public int result = 0;
}
public class MsgRoomInfo : MsgBase
{
    public MsgRoomInfo()
    {
        protoName = "MsgRoomInfo";
    }
    public RoomInfo roomInfo = new RoomInfo();
}
public class MsgLeaveRoom:MsgBase
{
    public MsgLeaveRoom()
    {
        protoName = "MsgLeaveRoom";
    }
    public string roomId;
    public int result = 0;
}
public class MsgStartGame:MsgBase
{
    public MsgStartGame()
    {
        protoName = "MsgStartGame";
    }
    public string roomId;
    public int result = 0;
}

[Serializable]
public class RoomInfo
{
    public string roomId;
    public List<string> playerIdList = new List<string>();
    public override string ToString()
    {
        return $"RoomID: {roomId}, Players: {string.Join(", ", playerIdList)}";
    }
}