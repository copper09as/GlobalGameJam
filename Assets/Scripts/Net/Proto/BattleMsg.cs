public class MsgMove : MsgBase
{
    public MsgMove()
    {
        protoName = "MsgMove";
    }
    public string id;
    public float x = 0;
    public float y = 0;
    public float angle = 0;
}
public class MsgPos:MsgBase
{
    public MsgPos()
    {
        protoName = "MsgPos";
    }
    public string id;
    public float x = 0;
    public float y = 0;
}
public class MsgHpChange : MsgBase
{
    public MsgHpChange()
    {
        protoName = "MsgHpChange";
    }
    public string id;
    public int hp = 0;
    public int MaxHp = 0;
}
public class MsgBulletChange : MsgBase
{
    public MsgBulletChange()
    {
        protoName = "MsgBulletChange";
    }
    public string id;
    public int CurrentBullet = 0;
    public int MaxBullet = 0;
}
public class MsgGameOver : MsgBase
{
    public MsgGameOver()
    {
        protoName = "MsgGameOver";
    }
    public string winnerId;
}
public class MsgReplacePos : MsgBase
{
    public MsgReplacePos()
    {
        protoName = "MsgReplacePos";
    }
    public string id;
    public float x;
    public float y;
}