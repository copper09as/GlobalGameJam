public class MsgBattleReady : MsgBase
{
    public MsgBattleReady()
    {
        protoName = "MsgBattleReady";
    }
}
public class MsgSyncState : MsgBase
{
    public MsgSyncState()
    {
        protoName = "MsgSyncState";
    }
        public float posX;
        public float posY;
        public float rot;
        public int hp;         // 服务器权威同步
        public int actionId;   // 当前动作，其他客户端需要
        public int bulletCount;
        public float timestamp;
}
public class MsgGameWin:MsgBase
{
    public MsgGameWin()
    {
        protoName = "MsgGameWin";
    }
}
public class MsgGameLose : MsgBase
{
    public MsgGameLose()
    {
        protoName = "MsgGameLose";
    }
}
public class MsgFire : MsgBase
{
    public MsgFire()
    {
        protoName = "MsgFire";
    }
    public string id;
    public float targetX;
    public float targetY;
    public float fireX;
    public float fireY;
}
public class MsgCreateMask: MsgBase
{
    public MsgCreateMask()
    {
        protoName = "MsgCreateMask";
    }
    public int MaskId = 0;
    public int EntityId = 0;
    public float posX;
    public float posY;
}
public class MsgTakeMask : MsgBase
{
    public MsgTakeMask()
    {
        protoName = "MsgTakeMask";
    }
    public int EntityId = 0;
    public string PlayerId = "";
}
public class MsgSwapPositionRequest : MsgBase
{
    public MsgSwapPositionRequest()
    {
        protoName = "MsgSwapPositionRequest";
    }
    public string TargetPlayerId; // 想交换位置的对象
}
public class MsgSwapPositionResponse : MsgBase
{
    public MsgSwapPositionResponse()
    {
        protoName = "MsgSwapPositionResponse";
    }
    public string PlayerAId;
    public float PlayerAX;
    public float PlayerAY;

    public string PlayerBId;
    public float PlayerBX;
    public float PlayerBY;
}
public class MsgShineEffect: MsgBase
{
    public MsgShineEffect()
    {
        protoName = "MsgShineEffect";
    }
}