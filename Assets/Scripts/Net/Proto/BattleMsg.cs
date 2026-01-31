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