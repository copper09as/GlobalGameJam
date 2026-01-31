using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : Player 
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DownLoad(context);
        Synchronization(context);
    }
    void DownLoad(PlayerContext context)
    {

    }
    /// <summary>
    /// 上下文数据同步到本地组件
    /// </summary>
    /// <param name="context"></param>
    void Synchronization(PlayerContext context)
    {
        transform.position = context.position;
        transform.rotation= context.rotation;
        transform.localScale = context.localScale;
    }
}
