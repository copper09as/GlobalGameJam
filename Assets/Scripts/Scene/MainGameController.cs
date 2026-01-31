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
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);
    }

    private void OnMsgLogin(MsgBase msgBase)
    {
        GameObject playerObj = Instantiate(Resources.Load<GameObject>("Prefabs/RemotePlayer"));
        Player player = playerObj.GetComponent<Player>();
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer = player;
    }

    private void OnMsgMove(MsgBase msgBase)
    {
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer.context.position = new Vector3(((MsgMove)msgBase).x, ((MsgMove)msgBase).y, 0);
    }

}
