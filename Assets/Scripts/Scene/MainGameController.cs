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
    [SerializeField] private GameObject maskPrefab;
    private NetSystem netSystem;
    private BattleContext battleContext;
    private SessionContext sessionContext;
    private EventSystem eventSystem;
    protected override void Awake()
    {
        base.Awake();
        netSystem = GameEntry.Instance.GetSystem<NetSystem>();
        eventSystem = GameEntry.Instance.GetSystem<EventSystem>();
        netSystem.AddMsgListener("MsgBattleReady", OnMsgBattleReady);
        netSystem.AddMsgListener("MsgSyncState", OnMsgSyncState);
        netSystem.AddMsgListener("MsgGameWin", OnMsgGameWin);
        netSystem.AddMsgListener("MsgFire", OnMsgFire);
        netSystem.AddMsgListener("MsgCreateMask", OnMsgCreateMask);
        netSystem.AddMsgListener("MsgGameLose", OnMsgGameLose);
        netSystem.AddMsgListener("MsgTakeMask", OnMsgTakeMask);
        netSystem.AddMsgListener("MsgShineEffect", OnMsgShineEffect);
        netSystem.AddMsgListener("MsgSwapPositionResponse", OnMsgSwapPosition);
        eventSystem.Subscribe<PlayerBulletChange>(OnPlayerBulletChange);
        eventSystem.Subscribe<PlayerHpChange>(OnPlayerHpChange);
        sessionContext = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>();
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<BattleContext>() != null)
        {
            GameEntry.Instance.GetSystem<ContextSystem>().DisposeContext<BattleContext>();
        }
        battleContext = GameEntry.Instance.GetSystem<ContextSystem>().CreateContext<BattleContext>();
        StartBattle();
    }

    private void OnPlayerBulletChange(PlayerBulletChange change)
    {
        int bulletCount = change.bulletCount;
        int maxBulletCount = change.MaxBulletCount;
        if (change.isLocalPlayer)
        {
            bulletBar.maxValue = maxBulletCount;
            bulletBar.SetValue(bulletCount);
        }
        else
        {
            syncBulletBar.maxValue = maxBulletCount;
            syncBulletBar.SetValue(bulletCount);
        }
    }

    private void OnMsgGameLose(MsgBase msgBase)
    {
        SceneManager.LoadScene("MainMenuScene");
        GameEntry.Instance.GetSystem<GlobalUiSystem>().ShowNotification("Game Over", "You Lose!");
    }

    private void OnPlayerHpChange(PlayerHpChange change)
    {
        int hp = change.hp;
        int maxHp = change.MaxHp;
        if (change.isLocalPlayer)
        {
            hpBar.maxValue = maxHp;
            hpBar.SetValue(hp);
        }
        else
        {
            syncHpBar.maxValue = maxHp;
            syncHpBar.SetValue(hp);
        }
    }

    private void OnMsgSyncState(MsgBase msgBase)
    {
        MsgSyncState msg = (MsgSyncState)msgBase;
        if (battleContext.SyncPlayer == null) return;
        Player syncPlayer = battleContext.SyncPlayer;
        syncPlayer.transform.position = new Vector2(msg.posX, msg.posY);
        syncPlayer.FirePoint.transform.parent.rotation = Quaternion.Euler(0f, 0f, msg.rot);
        syncPlayer.ChangeState((PlayerState)msg.actionId);
        syncPlayer.AbilitySystem.GetAttribute("Hp").
        AddModifier(new AttributeModifier
        ("Hp", ModifierOp.Add, msg.hp - syncPlayer.AbilitySystem.GetAttribute("Hp").Value));
        syncPlayer.AbilitySystem.GetAttribute("BulletCount").
        AddModifier(new AttributeModifier
        ("BulletCount", ModifierOp.Add, msg.bulletCount - syncPlayer.AbilitySystem.GetAttribute("BulletCount").Value));
    }

    protected override void Start()
    {
        base.Start();

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        netSystem.RemoveListener("MsgBattleReady", OnMsgBattleReady);
        netSystem.RemoveListener("MsgSyncState", OnMsgSyncState);
        netSystem.RemoveListener("MsgGameWin", OnMsgGameWin);
        netSystem.RemoveListener("MsgFire", OnMsgFire);
        netSystem.RemoveListener("MsgGameLose", OnMsgGameLose);
        netSystem.RemoveListener("MsgCreateMask", OnMsgCreateMask);
        netSystem.RemoveListener("MsgTakeMask", OnMsgTakeMask);
        netSystem.RemoveListener("MsgSwapPositionResponse", OnMsgSwapPosition);
        netSystem.RemoveListener("MsgShineEffect", OnMsgShineEffect);
        eventSystem.Unsubscribe<PlayerBulletChange>(OnPlayerBulletChange);
        eventSystem.Unsubscribe<PlayerHpChange>(OnPlayerHpChange);
    }

    private void OnMsgShineEffect(MsgBase msgBase)
    {
        StartCoroutine(ShineFlashThreeTimes());
    }

    private void OnMsgTakeMask(MsgBase msgBase)
    {
        MsgTakeMask msg = (MsgTakeMask)msgBase;
        var mask = battleContext.Masks.Find(m => m.EntityId == msg.EntityId);
        var player = battleContext.Players.Find(p => p.playerId == msg.PlayerId);
        battleContext.Masks.Remove(mask);
        if (mask != null && player != null)
        {
            mask.BeUsed(player);
        }
    }
    private void OnMsgSwapPosition(MsgBase msgBase)
    {
        MsgSwapPositionResponse msg = (MsgSwapPositionResponse)msgBase;

        Player playerA = battleContext.Players.Find(p => p.playerId == msg.PlayerAId);
        Player playerB = battleContext.Players.Find(p => p.playerId == msg.PlayerBId);
        Debug.Log($"Swapping positions of Player {msg.PlayerAId} and Player {msg.PlayerBId}");
        if (playerA != null)
            playerA.transform.position = new Vector2(msg.PlayerAX, msg.PlayerAY);
        if (playerB != null)
            playerB.transform.position = new Vector2(msg.PlayerBX, msg.PlayerBY);
    }
    private void OnMsgCreateMask(MsgBase msgBase)
    {
        MsgCreateMask msg = (MsgCreateMask)msgBase;
        GameObject maskObj = Instantiate(maskPrefab);
        var maskData = sessionContext.maskCollection.GetMaskSOById(msg.MaskId);
        var mask = maskObj.GetComponent<Mask>();
        mask.MaskSO = maskData;
        mask.EntityId = msg.EntityId;
        maskObj.transform.position = new Vector3(msg.posX, msg.posY, 0f);
        battleContext.Masks.Add(mask);
    }

    private void OnMsgFire(MsgBase msgBase)
    {
        MsgFire msg = (MsgFire)msgBase;
        if (battleContext.SyncPlayer == null) return;
        Player syncPlayer = battleContext.SyncPlayer;
        Vector3 targetPos = new Vector3(msg.targetX, msg.targetY, 0f);
        syncPlayer.CreateBullet(targetPos);
    }

    private void OnMsgGameWin(MsgBase msgBase)
    {
        GameEntry.Instance.GetSystem<GlobalUiSystem>().GameWin();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void StartBattle()
    {
        MsgBattleReady msg = new MsgBattleReady();
        netSystem.Send(msg);
    }

    private void OnMsgBattleReady(MsgBase msgBase)
    {

        GameObject playerObj = Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
        Player player = playerObj.GetComponent<Player>();
        battleContext.Players.Add(player);
        player.playerId = sessionContext.PlayeId;
        GameObject syncPlayerObj = Instantiate(Resources.Load<GameObject>("Prefabs/SyncPlayer"));
        Player syncPlayer = syncPlayerObj.GetComponent<Player>();
        syncPlayer.playerId = sessionContext.opponentId;
        battleContext.Players.Add(syncPlayer);
        battleContext.StartBattle();
    }
    private IEnumerator ShineFlashThreeTimes()
    {
        int times = 3;          // 闪烁次数
        float duration = 0.3f;  // 每次闪烁持续时间

        // 激活所有粒子系统对象
        foreach (var ps in shineEffect)
        {
            ps.gameObject.SetActive(true);
        }

        for (int i = 0; i < times; i++)
        {
            // 播放所有粒子系统
            foreach (var ps in shineEffect)
            {
                ps.Play();
            }
            yield return new WaitForSeconds(duration);

            // 停止所有粒子系统
            foreach (var ps in shineEffect)
            {
                ps.Stop();
            }
            yield return new WaitForSeconds(duration);
        }

        // 关闭所有粒子系统对象
        foreach (var ps in shineEffect)
        {
            ps.gameObject.SetActive(false);
        }
    }

}
