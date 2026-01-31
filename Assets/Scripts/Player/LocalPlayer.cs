using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class LocalPlayer : Player
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        Synchronization(data);
        UpLoad(data);
          if (Input.GetKey(KeyCode.A))
                {
                    transform.position += Vector3.left* Time.deltaTime*5;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.position += Vector3.right * Time.deltaTime * 5;

                    SendPlayerMsg();
    }
    void UpLoad(PlayerData data)
    {

    }
    /// <summary>
    /// ��������ͬ��������
    /// </summary>
    /// <param name="context"></param>
    void Synchronization(PlayerData data)
    {
        data.position = transform.position;
        data.rotation = transform.rotation;
        data.localScale = transform.localScale;
    }
    void SendPlayerMsg()
        {
            MsgMove msg = new MsgMove();
            msg.x = transform.position.x;
            msg.y = transform.position.y;
            NetManager.Send(msg);
        }
}
}
