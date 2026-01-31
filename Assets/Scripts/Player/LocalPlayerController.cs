using UnityEngine;

[CreateAssetMenu(menuName ="Player/LocalPlayerController")]
public class LocalPlayerController : ScPlayerController
{
    [SerializeField]private KeyCode upKey = KeyCode.W;
    [SerializeField]private KeyCode downKey = KeyCode.S;
    [SerializeField]private KeyCode leftKey = KeyCode.A;
    [SerializeField]private KeyCode rightKey = KeyCode.D;
    public override void Update(Player player, float deltaTime)
    {
        base.Update(player, deltaTime);
         

    }
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

    }
    void SendMoveMsg(Player player)
    {
        MsgMove msg = new MsgMove();
        msg.x = player.MoveDirection.x;
        msg.y = player.MoveDirection.y;
        msg.id = player.playerName;
        msg.angle = player.transform.rotation.eulerAngles.z;
        NetManager.Send(msg);
    }
}