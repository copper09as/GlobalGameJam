using GameFramework;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerController", menuName = "Player/Player Controller")]
public class PlayerController : ScriptableObject
{
    // 移动按键
    [Header("Keys")]
    [SerializeField] private KeyCode upKey = KeyCode.W;
    [SerializeField] private KeyCode downKey = KeyCode.S;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode fireKey = KeyCode.Space;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;



    public void Tick(Player player, float deltaTime)
    {
        // 处理移动
        ControlMove(player);

        // 旋转
        Rotate(player);

        // 射击
        Fire(player);

        // 重装
        Reload(player);

        SendSyncState(player);
        // 定时同步状态
        // 冷却时间累加
        player.currentColdDownTime += deltaTime;
    }

    #region Move & State
    public void ControlMove(Player player)
    {
        float x = 0f;
        float y = 0f;
        if (Input.GetKey(leftKey)) x = -1f;
        if (Input.GetKey(rightKey)) x = 1f;
        if (Input.GetKey(upKey)) y = 1f;
        if (Input.GetKey(downKey)) y = -1f;

        Vector2 moveDir = new Vector2(x, y).normalized;
        if (moveDir != Vector2.zero)
        {
            player.ChangeState(PlayerState.Move);
            player.Rb.velocity = moveDir * player.AbilitySystem.GetAttribute("MoveSpeed").Value;
        }
        else
        {
            player.ChangeState(PlayerState.Idle);
            player.Rb.velocity = Vector2.zero;
        }
    }
    #endregion

    #region Rotate
    public void Rotate(Player player)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector3 dir = mouseWorldPos - player.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        player.FirePoint.transform.parent.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    #endregion

    #region Fire
    public void Fire(Player player)
    {
        if ((Input.GetKeyDown(fireKey) ||Input.GetMouseButtonDown(0))&& player.AbilitySystem.GetAttribute("BulletCount").Value > 0)
        {
            if (player.currentColdDownTime >= player.AbilitySystem.GetAttribute("FireRate").Value && !player.InReload)
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0f;
                player.Fire(mouseWorldPos);
                player.currentColdDownTime = 0f;
                SendFireMsg(player, mouseWorldPos, player.FirePoint.transform.position);
                player.AbilitySystem.GetAttribute("BulletCount").AddModifier(new AttributeModifier
                ("BulletCount", ModifierOp.Add, -1));
            }
            if(player.AbilitySystem.GetAttribute("BulletCount").Value<=0)
            {
                player.StartReload();
            }
        }
    }
    #endregion

    #region Reload
    public void Reload(Player player)
    {
        if (Input.GetKeyDown(reloadKey) && !player.InReload)
        {
            player.StartReload();
        }
    }
    #endregion

    #region Networking
    private void SendSyncState(Player player)
    {
        MsgSyncState msg = new MsgSyncState
        {

            posX = player.transform.position.x,
            posY = player.transform.position.y,
            rot = player.FirePoint.transform.parent.rotation.eulerAngles.z,
            hp = (int)player.AbilitySystem.GetAttribute("Hp").Value,
            bulletCount = (int)player.AbilitySystem.GetAttribute("BulletCount").Value,
            actionId = (int)player.CurrentState,
            timestamp = Time.time
        };

        GameEntry.Instance.GetSystem<NetSystem>().Send(msg);
    }

    private void SendFireMsg(Player player, Vector3 targetPosition, Vector3 firePosition)
    {
        MsgFire msg = new MsgFire
        {
            id = player.playerId,
            targetX = targetPosition.x,
            targetY = targetPosition.y,
            fireX = firePosition.x,
            fireY = firePosition.y
        };
        GameEntry.Instance.GetSystem<NetSystem>().Send(msg);
    }


    #endregion
}
