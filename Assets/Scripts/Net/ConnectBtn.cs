using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectBtn : MonoBehaviour
{
    public void Register()
    {
        MsgLogin msg = new MsgLogin();
        //等于显卡名字
        msg.id = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("Device ID: " + msg.id);
        msg.pw = "password1";
        NetManager.Send(msg);
    }
}
