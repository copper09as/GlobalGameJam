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
    [SerializeField]private ParticleSystem shineEffect;
    [SerializeField] private ProgressBar bulletBar;
    [SerializeField] private ProgressBar hpBar;
    [SerializeField] private ProgressBar syncHpBar;
    [SerializeField] private ProgressBar syncBulletBar;
    [SerializeField] private Image mask;
    [SerializeField] private Image syncMask;
    
    private Dictionary<string,Action<string>> maskEffectDict;
        protected override void Awake()
    {
        base.Awake();
        maskEffectDict = new();
        maskEffectDict.Add("ShineMask", ShineEffect);
        maskEffectDict.Add("ReplacePosMask", ReplacePosEffect);
        maskEffectDict.Add("EvilMask", EvilEffect);
        NetManager.Connect("139.9.116.94", 7777);
        NetManager.AddMsgListener("MsgEvil",OnMsgEvil);
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);
        NetManager.AddMsgListener("MsgLoadPlayer", OnMsgPlayerLoad);
        NetManager.AddMsgListener("MsgCreateBullet", OnMsgCreateBullet);
        NetManager.AddMsgListener("MsgPos", OnSyncPosition);
        NetManager.AddMsgListener("MsgHpChange", OnMsgHpChange);
        NetManager.AddMsgListener("MsgBulletChange", OnMsgBulletChange);
        NetManager.AddMsgListener("MsgReplacePos", OnMsgReplacePos);
        NetManager.AddMsgListener("MsgShine", OnMsgShine);
        
        GameEntry.Instance.GetSystem<EventSystem>().Subscribe<PlayerEvent.UseMaskEffect>(TrigMaskEffect);
        NetManager.AddEventListener(NetEvent.Close,OnClose);
        GameEntry.Instance.GetSystem<EventSystem>().Subscribe<PlayerEvent.PlayerBulletChange>(BulletChange);
        GameEntry.Instance.GetSystem<EventSystem>().Subscribe<PlayerEvent.PlayerHpChange>(HpChange);
        GameEntry.Instance.GetSystem<EventSystem>().Subscribe<PlayerEvent.PlayerMaskChange>(MaskChange);
        GameObject playerObj = Instantiate(Resources.Load<GameObject>("Prefabs/LocalPlayer"));
        Player player = playerObj.GetComponent<Player>();
        if(GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>()==null)
        {
            GameEntry.Instance.GetSystem<ContextSystem>().CreateContext<SessionContext>();
            GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().PlayerName = "Player"+(DateTime.Now.Ticks*31279%10000).ToString();
        }
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer = player;
        player.playerName = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().PlayerName;
        player.name = player.playerName;
        InvokeRepeating(nameof(SyncPosition),1f,2f);
    }

    private void OnMsgEvil(MsgBase msgBase)
    {
        MsgEvil msg = msgBase as MsgEvil;
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer == null)
        {
            return;
        }
        if(msg.id== GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        syncMask.sprite = GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().maskCollection.GetMaskSOByName("EvilMask").Sprite;
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().SyncPlayer.currentMaskName = "EvilMask";
    }

    private void OnMsgReplacePos(MsgBase msgBase)
{
    MsgReplacePos msg = msgBase as MsgReplacePos;
    var session = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>();

    // 确保消息不是自己发的
    if(session.LocalPlayer.playerName != msg.id) 
    {
        // 自己被目标玩家换位置了
        Vector3 originPos = session.LocalPlayer.transform.position;
        session.LocalPlayer.transform.position = new Vector3(msg.x, msg.y, 0);

        // 如果远程玩家有 SyncPlayer，也把远程位置交换（可选）
        if(session.SyncPlayer != null)
        {
            session.SyncPlayer.transform.position = originPos;
            
        }
        var data = GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().maskCollection.GetMaskSOByName("ReplacePosMask");
        syncMask.sprite = data.Sprite;
        SyncPosition();
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().SyncPlayer.currentMaskName = "ReplacePosMask";
        return;
    }
        var ldata = GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().maskCollection.GetMaskSOByName("ReplacePosMask");
        mask.sprite = ldata.Sprite;
}


    private void OnClose(string err)
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    protected override void Start()
    {
        base.Start();   
        MsgLogin msg = new MsgLogin();
        msg.id = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName;
        NetManager.Send(msg);
    }

    private void OnMsgBulletChange(MsgBase msgBase)
    {
        MsgBulletChange msg = msgBase as MsgBulletChange;
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer == null)
        {
            return;
        }
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().SyncPlayer.BulletCount.Value = msg.CurrentBullet;
        syncBulletBar.maxValue = msg.MaxBullet;
        syncBulletBar.SetValue(msg.CurrentBullet);
    }

    private void OnMsgHpChange(MsgBase msgBase)
    {
        MsgHpChange msg = msgBase as MsgHpChange;
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer == null)
        {
            return;
        }
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().SyncPlayer.Hp.Value = msg.hp;
        syncHpBar.maxValue = msg.MaxHp;
        syncHpBar.SetValue(msg.hp);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameEntry.Instance.GetSystem<EventSystem>().Unsubscribe<PlayerEvent.PlayerBulletChange>(BulletChange);
        GameEntry.Instance.GetSystem<EventSystem>().Unsubscribe<PlayerEvent.PlayerHpChange>(HpChange);
        NetManager.RemoveListener("MsgLogin", OnMsgLogin);
        NetManager.RemoveListener("MsgMove", OnMsgMove);
        NetManager.RemoveListener("MsgLoadPlayer", OnMsgPlayerLoad);
        NetManager.RemoveListener("MsgCreateBullet", OnMsgCreateBullet);
        NetManager.RemoveListener("MsgPos", OnSyncPosition);
        NetManager.RemoveListener("MsgHpChange", OnMsgHpChange);
        NetManager.RemoveListener("MsgBulletChange", OnMsgBulletChange);
        NetManager.RemoveListener("MsgReplacePos", OnMsgReplacePos);
        NetManager.RemoveListener("MsgShine", OnMsgShine);
        NetManager.RemoveListener("MsgEvil",OnMsgEvil);
        NetManager.RemoveEventListener(NetEvent.Close,OnClose);

        GameEntry.Instance.GetSystem<EventSystem>().Unsubscribe<PlayerEvent.UseMaskEffect>(TrigMaskEffect);
        NetManager.RemoveEventListener(NetEvent.Close,OnClose);
    }

    private void OnMsgShine(MsgBase msgBase)
    {
       if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer == null)
        {
            return;
        }
        MsgShine msg = msgBase as MsgShine;
        var maskData =
         GameEntry.Instance.GetSystem<ContextSystem>().
         GetContext<SessionContext>().maskCollection.GetMaskSOByName("ShineMask");
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            mask.sprite = maskData.Sprite;
            return;
        }
        Debug.Log("收到闪耀特效消息");
        syncMask.sprite =maskData.Sprite;
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().SyncPlayer.currentMaskName = "ShineMask";
        StartCoroutine(ShineFlashThreeTimes());
    }
    private IEnumerator ShineFlashThreeTimes()
    {
        int times = 3;        // 闪烁次数
        float duration = 0.3f; // 每次闪烁持续时间
        shineEffect.gameObject.SetActive(true);
        for(int i = 0; i < times; i++)
        {
            shineEffect.Play();     // 开始播放
            yield return new WaitForSeconds(duration);

            shineEffect.Stop();     // 停止播放
            yield return new WaitForSeconds(duration);
        }
        shineEffect.gameObject.SetActive(false);
    }
    private void HpChange(PlayerHpChange change)
    {
        if(change.id== GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            hpBar.maxValue = change.MaxHp;
            hpBar.SetValue(change.hp);
            return;
        }
        syncHpBar.maxValue = change.MaxHp;
        syncHpBar.SetValue(change.hp);
    }
    private void MaskChange(PlayerMaskChange change)
    {
        
        if (change.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            mask.sprite = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().maskCollection.GetMaskSOByName(change.MaskName).Sprite;
            return;
        }
        
    }
    private void OnMsgCreateBullet(MsgBase msgBase)
    {
        MsgCreateBullet msg = msgBase as MsgCreateBullet;
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer == null)
        {
            return;
        }
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().SyncPlayer.
        CreateBullet(new Vector3(msg.targetX, msg.targetY, 0), new Vector3(msg.fireX, msg.fireY, 0),true);
    }

    private void OnMsgPlayerLoad(MsgBase msgBase)
    {
        MsgLoadPlayer msg = msgBase as MsgLoadPlayer;
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        GameObject playerObj;
        string path;
        if(GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().playerType==1)
        {
       
            path = "Prefabs/P1";
        }
        else
        {
    
            path = "Prefabs/P2";
        }
        playerObj = Instantiate(Resources.Load<GameObject>(path));
        Player player = playerObj.GetComponent<Player>();
        player.playerName = msg.id;
        player.startGame = true;
        Sun.Instance.SunStart();
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.startGame = true;
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer = player;
        player.controller = Resources.Load<RemotePlayerController>("Prefabs/NewRemotePlayerController");
    }

    private void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msg = msgBase as MsgLogin;
        Debug.Log("收到登录返回消息，结果：" + msg.id);
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }

        GameObject playerObj;
        string path;
        if(msg.playerType==1)
        {
              GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().playerType = 1;
            path = "Prefabs/P1";
        }
        else
        {
            GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().playerType = 2;
            path = "Prefabs/P2";
        }
        playerObj = Instantiate(Resources.Load<GameObject>(path));
        Player player = playerObj.GetComponent<Player>();
        player.playerName = msg.id;
        player.startGame = true;
        Sun.Instance.SunStart();
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.startGame = true;
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer = player;
        player.controller = Resources.Load<RemotePlayerController>("Prefabs/NewRemotePlayerController");
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer = player;
        MsgLoadPlayer loadMsg = new MsgLoadPlayer();
        loadMsg.id = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName;
        NetManager.Send(loadMsg);
    }
    private void OnMsgMove(MsgBase msgBase)
    {
        MsgMove msg = msgBase as MsgMove;
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer == null)
        {
            return;
        }
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer.MoveDirection = new Vector2(msg.x, msg.y);
        GameEntry.Instance.GetSystem<ContextSystem>().
GetContext<SessionContext>().SyncPlayer.FirePoint.transform.parent.rotation = Quaternion.Euler(0f, 0f, msg.angle);

    }
    private void OnSyncPosition(MsgBase msgBase)
    {
        MsgPos msg = msgBase as MsgPos;
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer == null)
        {
            return;
        }
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().SyncPlayer.
        transform.position = new Vector3(msg.x, msg.y, 0);
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().SyncPlayer.Rb.velocity = Vector2.zero;
    }
    public void BulletChange(PlayerEvent.PlayerBulletChange evt)
    {
        if (bulletBar == null) return;
        bulletBar.maxValue=evt.MaxBullet;
        bulletBar.SetValue(evt.CurrentBullet);
    }
    private void SyncPosition()
    {
        var player = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer;
        MsgPos msgPos = new MsgPos();
        msgPos.id = player.playerName;
        msgPos.x = player.transform.position.x;
        msgPos.y = player.transform.position.y;
        NetManager.Send(msgPos);
    }
    public void TrigMaskEffect(PlayerEvent.UseMaskEffect evt)
    {
        string id = evt.id;
        string maskName = evt.MaskName;
        if(maskEffectDict.ContainsKey(maskName))
        {
            maskEffectDict[maskName].Invoke(id);
        }
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().LocalPlayer.currentMaskName = maskName;
         
    }
    

    
    #region 面具效果
    private void ShineEffect(string id)
    {
        Debug.Log("捡起闪耀面具，触发闪耀特效");
        MsgShine msg = new MsgShine();
        msg.id = id;
        NetManager.Send(msg);
    }
private void ReplacePosEffect(string targetId)
{
    var session = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>();
    var localPlayer = session.LocalPlayer;
    var syncPlayer = session.SyncPlayer;

    // 记录自己位置
    Vector3 originPos = localPlayer.transform.position;

    // 发送消息给对方（msg.x/y = 自己位置，id = 对方）
    MsgReplacePos msg = new MsgReplacePos();
    msg.id = targetId; // 目标玩家 id
    msg.x = originPos.x;
    msg.y = originPos.y;
    NetManager.Send(msg);

    // 本地立即交换
    if(syncPlayer != null)
    {
        localPlayer.transform.position = syncPlayer.transform.position;
        syncPlayer.transform.position = originPos;
    }
}

    private void EvilEffect(string obj)
    {
        MsgEvil msgEvil = new MsgEvil();
        msgEvil.id = obj;
        NetManager.Send(msgEvil);
        Debug.Log("触发邪恶面具效果");
    }

    #endregion

}
