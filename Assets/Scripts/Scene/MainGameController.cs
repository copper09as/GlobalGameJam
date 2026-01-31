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
        if(GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>() == null)
        {
            GameEntry.Instance.GetSystem<ContextSystem>().CreateContext<SessionContext>();
        }
        MsgLogin msg = new MsgLogin();
            //  等于显卡名字
         msg.id = SystemInfo.deviceUniqueIdentifier;
         Debug.Log("Device ID: " + msg.id);
        msg.pw = "password1";
        NetManager.Send(msg);
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);
    }

    private void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msg = msgBase as MsgLogin;
        if(msg.id== SystemInfo.deviceUniqueIdentifier)
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
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer.context.position = new Vector3(((MsgMove)msgBase).x, ((MsgMove)msgBase).y, 0);
    }

}
