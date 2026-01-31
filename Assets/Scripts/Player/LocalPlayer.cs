using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using static Player;

public class LocalPlayer : Player
{

    // Update is called once per frame
    protected override void Update()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) y = 1f;
        if (Input.GetKey(KeyCode.S)) y = -1f;

        Vector2 moveDir = new Vector2(x, y).normalized;

        rb.velocity = moveDir * moveSpeed;
        if(Input.GetMouseButtonDown(0))
        {

            // 鼠标屏幕坐标 → 世界坐标
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            // 计算方向（并归一化）
            Vector3 dir = (mouseWorldPos - transform.position).normalized;
            CreateBullet(dir);
            MsgCreateBullet msg = new MsgCreateBullet();
            msg.id = playerName;
            msg.targetX = mouseWorldPos.x;
            msg.targetY = mouseWorldPos.y;
            NetManager.Send(msg);
        }
         base.Update();
        SendPlayerMsg();
    }
    void SendPlayerMsg()
    {
        MsgMove msg = new MsgMove();
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.id = playerName;
        NetManager.Send(msg);
    }
}
