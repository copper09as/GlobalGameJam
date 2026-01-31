using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class LocalPlayer : Player
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left* Time.deltaTime*5;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * Time.deltaTime * 5;

    }
        SendPlayerMsg();
    }
    void SendPlayerMsg()
    {
        MsgMove msg = new MsgMove();
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        NetManager.Send(msg);
    }
}
