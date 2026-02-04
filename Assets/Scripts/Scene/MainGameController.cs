using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using Michsky.MUIP;
using PlayerEvent;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameController : GameBehaviour
{
    [SerializeField] private List<ParticleSystem> shineEffect;
    [SerializeField] private ProgressBar bulletBar;
    [SerializeField] private ProgressBar hpBar;
    [SerializeField] private ProgressBar syncHpBar;
    [SerializeField] private ProgressBar syncBulletBar;
    [SerializeField] private Image mask;
    [SerializeField] private Image syncMask;
    private NetSystem netSystem;
    private BattleContext battleContext;
    private SessionContext sessionContext;
    private Dictionary<string, Action<string>> maskEffectDict;
    protected override void Awake()
    {
        base.Awake();
        netSystem = GameEntry.Instance.GetSystem<NetSystem>();
        netSystem.AddMsgListener("MsgBattleReady", OnMsgBattleReady);
        if(GameEntry.Instance.GetSystem<ContextSystem>().GetContext<BattleContext>() != null)
        {
            GameEntry.Instance.GetSystem<ContextSystem>().DisposeContext<BattleContext>();
        }
        battleContext = GameEntry.Instance.GetSystem<ContextSystem>().CreateContext<BattleContext>();

    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        netSystem.RemoveListener("MsgBattleReady", OnMsgBattleReady);
    }

    private void OnMsgBattleReady(MsgBase msgBase)
    {
        
    }
}
