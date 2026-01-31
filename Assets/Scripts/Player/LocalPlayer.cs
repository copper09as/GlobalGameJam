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
    }
    void UpLoad(PlayerData data)
    {

    }
    /// <summary>
    /// 本地数据同步到线上
    /// </summary>
    /// <param name="context"></param>
    void Synchronization(PlayerData data)
    {
        data.position = transform.position;
        data.rotation = transform.rotation;
        data.localScale = transform.localScale;
    }
}
