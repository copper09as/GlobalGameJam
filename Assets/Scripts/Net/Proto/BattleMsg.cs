public class MsgMove : MsgBase
{
    public MsgMove()
    {
        protoName = "MsgMove";
    }
    public string id;
    public float x = 0;
    public float y = 0;
}
public class MsgRotate : MsgBase
{
    public MsgRotate()
    {
        protoName = "MsgRotate";
    }
    public string id;
    public float angle = 0;
}