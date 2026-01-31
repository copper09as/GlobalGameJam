using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class LocalPlayer : Player
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Synchronization(context);
        UpLoad(context);
    }
    void UpLoad(PlayerContext context)
    {

    }
    /// <summary>
    /// 本地组件数据同步到上下文
    /// </summary>
    /// <param name="context"></param>
    void Synchronization(PlayerContext context)
    {
        context.position = transform.position;
        context.rotation = transform.rotation;
        context.localScale = transform.localScale;
    }
}
