using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

public class MainGameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);
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
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer = player;
    }

    private void OnMsgMove(MsgBase msgBase)
    {
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer == null)
        {
            return;
        }
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer.data.position = new Vector3(((MsgMove)msgBase).x, ((MsgMove)msgBase).y, 0);
    }

}
