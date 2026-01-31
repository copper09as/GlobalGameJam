public class MsgCreateBullet : MsgBase
{
    public MsgCreateBullet()
    {
        protoName = "MsgCreateBullet";
    }
    public string id = "";
    public float targetX = 0;
    public float targetY = 0;
}