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
    [SerializeField] private ProgressBar bulletBar;
    [SerializeField] private ProgressBar hpBar;
    [SerializeField] private ProgressBar syncHpBar;
    [SerializeField] private ProgressBar syncBulletBar;
    [SerializeField] private Image mask;
    [SerializeField] private Image syncMask;
    
    private Dictionary<string,Action> maskEffectDict = new Dictionary<string, Action>();
    protected override void Awake()
    {
        base.Awake();
        NetManager.Connect("139.9.116.94",7777);
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);
        NetManager.AddMsgListener("MsgLoadPlayer", OnMsgPlayerLoad);
        NetManager.AddMsgListener("MsgCreateBullet", OnMsgCreateBullet);
        NetManager.AddMsgListener("MsgPos", OnSyncPosition);
        NetManager.AddMsgListener("MsgHpChange", OnMsgHpChange);
        NetManager.AddMsgListener("MsgBulletChange", OnMsgBulletChange);
        NetManager.AddMsgListener("MsgGameOver", OnMsgGameOver);
        NetManager.AddMsgListener("MsgReplacePos", OnMsgReplacePos);
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

    private void OnMsgReplacePos(MsgBase msgBase)
    {
        MsgReplacePos msg = msgBase as MsgReplacePos;
        if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer == null)
        {
            return;
        }
        if (msg.id == GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            return;
        }
        var originPos = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer.transform.position;
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().SyncPlayer.transform.position = new Vector3(msg.x, msg.y, 0);
        GameEntry.Instance.GetSystem<ContextSystem>().
        GetContext<SessionContext>().LocalPlayer.transform.position = originPos;
        
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
    private void OnMsgGameOver(MsgBase msgBase)
    {
        MsgGameOver msg = msgBase as MsgGameOver;
        string result = "你输了！";
        if(msg.winnerId== GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            result = "你赢了！";
        }
        GameEntry.Instance.GetSystem<GlobalUiSystem>().ShowNotification("游戏结束",result);
        NetManager.Close();
        SceneManager.LoadScene("MainUiScene");
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
        NetManager.RemoveEventListener(NetEvent.Close,OnClose);
    }
    private void HpChange(PlayerHpChange change)
    {
        if(change.id== GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.playerName)
        {
            hpBar.maxValue = change.MaxHp;
            hpBar.SetValue(change.hp);
            if(change.hp<=0)
            {
                MsgGameOver msg = new MsgGameOver();
                msg.winnerId = change.id;
                NetManager.Send(msg);
            }
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
        GameObject playerObj = Instantiate(Resources.Load<GameObject>("Prefabs/LocalPlayer"));
        Player player = playerObj.GetComponent<Player>();
        player.playerName = msg.id;
        player.startGame = true;
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
        GameObject playerObj = Instantiate(Resources.Load<GameObject>("Prefabs/LocalPlayer"));
        Player player = playerObj.GetComponent<Player>();
        player.playerName = msg.id;
        player.startGame = true;
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
    #region 面具效果
    private void ShineEffect(string id)
    {
        
    }
    private void ReplacePosEffect(string id)
    {
        MsgReplacePos msg = new MsgReplacePos();
        var originPos = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.transform.position;
        msg.id = id;
        msg.x = originPos.x;
        msg.y = originPos.y;
        NetManager.Send(msg);
        transform.position = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer.transform.position;
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer.transform.position = originPos;
    }
    #endregion

}
