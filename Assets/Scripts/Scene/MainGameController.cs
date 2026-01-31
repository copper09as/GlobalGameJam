using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using TMPro;
using UnityEngine;

public class MainGameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);
        NetManager.AddMsgListener("MsgLoadPlayer", OnMsgPlayerLoad);
        NetManager.AddMsgListener("MsgCreateBullet", OnMsgCreateBullet);
        if(GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>() == null)
        {
            GameEntry.Instance.GetSystem<ContextSystem>().CreateContext<SessionContext>();
        }
        GameObject playerObj = Instantiate(Resources.Load<GameObject>("Prefabs/LocalPlayer"));
        Player player = playerObj.GetComponent<Player>();
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer = player;
        MsgLogin msg = new MsgLogin();
       //等于独立设备id
        msg.id = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName; 
        NetManager.Send(msg);

    }

    private void OnMsgCreateBullet(MsgBase msgBase)
    {
        MsgCreateBullet msg = msgBase as MsgCreateBullet;
        if(msg.id== GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().SyncPlayer.
        CreateBullet(new Vector3(msg.targetX, msg.targetY,0), new Vector3(msg.fireX, msg.fireY,0));
    }

    private void OnMsgPlayerLoad(MsgBase msgBase)
    {
        MsgLoadPlayer msg = msgBase as MsgLoadPlayer;
        if(msg.id== GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
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
        if(msg.id== GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
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
        Debug.Log(msg.id);
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer == null)
        {
            return;
        }
        if(msg.id== GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer.transform.position = new Vector3(msg.x, msg.y, 0);
    }

}
