using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : Player 
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }


    protected override void Update()
    {
        DownLoad(data);
        Synchronization(data);
    }
    void DownLoad(PlayerData data)
    {

    }
    /// <summary>
    /// 线上数据同步到本地组件
    /// </summary>
    /// <param name="context"></param>
    void Synchronization(PlayerData data)
    {
        transform.position = data.position;
        transform.rotation= data.rotation;
        transform.localScale = data.localScale;
    }
}
