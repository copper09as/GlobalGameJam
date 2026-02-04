using GameFramework;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/LocalPlayerController")]
public class LocalPlayerController : ScPlayerController
{
    [SerializeField] private KeyCode upKey = KeyCode.W;
    [SerializeField] private KeyCode downKey = KeyCode.S;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode fireKey = KeyCode.Space;
    [SerializeField]private KeyCode reloadKey = KeyCode.R;
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
    public override void Tick(Player player, float deltaTime)
    {
        player.currentColdDownTime += deltaTime;
    }
    public override void SetPosition(Player player, Vector2 position)
    {
        player.transform.position = position;
    }
    public override void Rotate(Player player)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector3 dir = mouseWorldPos - player.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        player.FirePoint.transform.parent.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    public override void Fire(Player player)
    {
        if (Input.GetKeyDown(fireKey) || Input.GetMouseButtonDown(0))
        {
            if(player.currentColdDownTime < player.coldDownTime)
            {
                return;
            }
            if(player.BulletCount.Value<=0)
            {
                return;
            }
            player.currentColdDownTime = 0f;
            player.BulletCount.Value -= 1;
            SendBulletMsg(player);
            GameEntry.Instance.GetSystem<EventSystem>().Publish(new PlayerEvent.PlayerBulletChange {
                CurrentBullet = player.BulletCount.Value,
                MaxBullet = player.MaxBullet,
                id = player.playerName
            });
            if(player.BulletCount.Value==0)
            {
                player.StartReload();
            }
            player.CreateBullet(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            SendFireMsg(player, Camera.main.ScreenToWorldPoint(Input.mousePosition), player.FirePoint.position);
        }
    }
    public override void Reload(Player player)
    {
        base.Reload(player);
        if(Input.GetKeyDown(reloadKey))
        {
            if(player.BulletCount.Value!=0)
            {
                player.StartReload();
                SendBulletMsg(player);
            }
        }
    }
    void SendBulletMsg(Player player)
    {
        MsgBulletChange msg = new MsgBulletChange();
        msg.id = player.playerName;
        msg.CurrentBullet = player.BulletCount.Value;
        msg.MaxBullet = player.MaxBullet;
        //NetManager.Send(msg);
    }
    void SendMoveMsg(Player player)
    {
        MsgMove msg = new MsgMove();
        msg.x = player.MoveDirection.x;
        msg.y = player.MoveDirection.y;
        msg.id = player.playerName;
        msg.angle = player.FirePoint.transform.parent.rotation.eulerAngles.z;
        //NetManager.Send(msg);
    }
    void SendFireMsg(Player player, Vector3 targetPosition, Vector3 firePosition)
    {
        MsgCreateBullet msg = new MsgCreateBullet();
        msg.id = player.playerName;
        msg.targetX = targetPosition.x;
        msg.targetY = targetPosition.y;
        msg.fireX = firePosition.x;
        msg.fireY = firePosition.y;
        //NetManager.Send(msg);
    }
}
