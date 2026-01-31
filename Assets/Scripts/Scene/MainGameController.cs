using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using Michsky.MUIP;
using TMPro;
using UnityEditor;
using UnityEngine;

public class MainGameController : GameBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ProgressBar bulletBar;
    protected override void Start()
    {
        base.Start();
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);
        NetManager.AddMsgListener("MsgLoadPlayer", OnMsgPlayerLoad);
        NetManager.AddMsgListener("MsgCreateBullet", OnMsgCreateBullet);
        NetManager.AddMsgListener("MsgPos", OnSyncPosition);
        GameEntry.Instance.GetSystem<EventSystem>().Subscribe<PlayerEvent.PlayerBulletChange>(BulletChange);
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>() == null)
        {
            GameEntry.Instance.GetSystem<ContextSystem>().CreateContext<SessionContext>();
        }
        GameObject playerObj = Instantiate(Resources.Load<GameObject>("Prefabs/LocalPlayer"));
        Player player = playerObj.GetComponent<Player>();
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer = player;
        MsgLogin msg = new MsgLogin();
        msg.id = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName;
        NetManager.Send(msg);
        InvokeRepeating(nameof(SyncPosition),1f,2f);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameEntry.Instance.GetSystem<EventSystem>().Unsubscribe<PlayerEvent.PlayerBulletChange>(BulletChange);
        NetManager.RemoveListener("MsgLogin", OnMsgLogin);
        NetManager.RemoveListener("MsgMove", OnMsgMove);
        NetManager.RemoveListener("MsgLoadPlayer", OnMsgPlayerLoad);
        NetManager.RemoveListener("MsgCreateBullet", OnMsgCreateBullet);
        NetManager.RemoveListener("MsgPos", OnSyncPosition);
    }
    private void OnMsgCreateBullet(MsgBase msgBase)
    {
        MsgCreateBullet msg = msgBase as MsgCreateBullet;
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer == null)
        {
            return;
        }
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().SyncPlayer.
        CreateBullet(new Vector3(msg.targetX, msg.targetY, 0), new Vector3(msg.fireX, msg.fireY, 0));
    }

    private void OnMsgPlayerLoad(MsgBase msgBase)
    {
        MsgLoadPlayer msg = msgBase as MsgLoadPlayer;
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        GameObject playerObj = Instantiate(Resources.Load<GameObject>("Prefabs/RemotePlayer"));
        Player player = playerObj.GetComponent<Player>();
        player.playerName = msg.id;
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer = player;
    }

    private void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msg = msgBase as MsgLogin;
        Debug.Log("收到登录返回消息，结果：" + msg.id);
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        GameObject playerObj = Instantiate(Resources.Load<GameObject>("Prefabs/RemotePlayer"));
        Player player = playerObj.GetComponent<Player>();
        player.playerName = msg.id;
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer = player;
        MsgLoadPlayer loadMsg = new MsgLoadPlayer();
        loadMsg.id = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName;
        NetManager.Send(loadMsg);
    }
    private void OnMsgMove(MsgBase msgBase)
    {
        MsgMove msg = msgBase as MsgMove;
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer == null)
        {
            return;
        }
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer.MoveDirection = new Vector2(msg.x, msg.y);
        GameEntry.Instance.GetSystem<ContextSystem>().
GetContext<SessionContext>().SyncPlayer.
transform.rotation = Quaternion.Euler(0f, 0f, msg.angle);

    }
    private void OnSyncPosition(MsgBase msgBase)
    {
        MsgPos msg = msgBase as MsgPos;
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer == null)
        {
            return;
        }
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().SyncPlayer.
        transform.position = new Vector3(msg.x, msg.y, 0);
    }
    public void BulletChange(PlayerEvent.PlayerBulletChange evt)
    {
        bulletBar.SetValue((float)evt.CurrentBullet / evt.MaxBullet * 100f);
    }
    private void SyncPosition()
    {
        var player = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer;
        MsgPos msgPos = new MsgPos();
        msgPos.id = player.playerName;
        msgPos.x = player.transform.position.x;
        msgPos.y = player.transform.position.y;
        NetManager.Send(msgPos);
    }

}
