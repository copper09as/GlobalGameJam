using UnityEngine;

[CreateAssetMenu(menuName ="Player/LocalPlayerController")]
public class LocalPlayerController : ScPlayerController
{
    [SerializeField]private KeyCode upKey = KeyCode.W;
    [SerializeField]private KeyCode downKey = KeyCode.S;
    [SerializeField]private KeyCode leftKey = KeyCode.A;
    [SerializeField]private KeyCode rightKey = KeyCode.D;
    
    private float lastRotateSendTime = 0f;
    [SerializeField]private float rotateSendInterval = 0.05f; // 每 50ms 发送一次旋转信息

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
        // 限制旋转消息的发送频率，避免网络拥塞
        if (Time.time - lastRotateSendTime < rotateSendInterval)
        {
            return;
        }
        
        MsgRotate msg = new MsgRotate();
        msg.angle = player.transform.rotation.eulerAngles.z;
        msg.id = player.playerName;
        NetManager.Send(msg);
        lastRotateSendTime = Time.time;
    }
}