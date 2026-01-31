public class MsgCreateBullet : MsgBase
{
    public MsgCreateBullet()
    {
        protoName = "MsgCreateBullet";
    }
    public string id = "";
    public float targetX = 0;
    public float targetY = 0;
    public float fireX = 0;
    public float fireY = 0;
}