using UnityEngine;

public class ScPlayerController:ScriptableObject
{
    public virtual void ControlMove(Player player)
    {

    }
    public virtual void SetPosition(Player player,Vector2 position)
    {

    }
    public virtual void Rotate(Player player)
    {

    }
    public virtual void Rotate(Player player, float angle)
    {
    }
}
[CreateAssetMenu(menuName ="Player/RemotePlayerController")]
public class RemotePlayerController : ScPlayerController
{
    public override void SetPosition(Player player,Vector2 position)
    {
        player.transform.position = position;
    }
    public override void Rotate(Player player, float angle)
    {
        player.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
[CreateAssetMenu(menuName ="Player/LocalPlayerController")]
public class LocalPlayerController : ScPlayerController
{
    [SerializeField]private KeyCode upKey = KeyCode.W;
    [SerializeField]private KeyCode downKey = KeyCode.S;
    [SerializeField]private KeyCode leftKey = KeyCode.A;
    [SerializeField]private KeyCode rightKey = KeyCode.D;

    public override void ControlMove(Player player)
    {
        float x = 0f;
        float y = 0f;
        if (Input.GetKey(leftKey)) x = -1f;
        if (Input.GetKey(rightKey)) x = 1f;
        if (Input.GetKey(upKey)) y = 1f;
        if (Input.GetKey(downKey)) y = -1f;
        player.MoveDirection = new Vector2(x, y);
        SendMoveMsg(player);
    }
    public override void SetPosition(Player player,Vector2 position)
    {
        player.transform.position = position;
    }
    public override void Rotate(Player player)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3 dir = mouseWorldPos - player.transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        player.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        SendRotateMsg(player);
    }
    void SendMoveMsg(Player player)
    {
        MsgMove msg = new MsgMove();
        msg.x = player.MoveDirection.x;
        msg.y = player.MoveDirection.y;
        msg.id = player.playerName;
        NetManager.Send(msg);
    }
    void SendRotateMsg(Player player)
    {
        MsgRotate msg = new MsgRotate();
        msg.angle = player.transform.rotation.eulerAngles.z;
        msg.id = player.playerName;
        NetManager.Send(msg);
    }
}