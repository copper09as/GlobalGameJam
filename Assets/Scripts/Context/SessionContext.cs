using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

public class SessionContext : GameContext
{
    protected override void OnInitialize()
    {
        // 会话初始化逻辑
        Debug.Log("[SessionContext] 会话上下文初始化");
    }

    protected override void OnDispose()
    {
        // 会话清理逻辑
        Debug.Log("[SessionContext] 会话上下文清理");
    }
}
