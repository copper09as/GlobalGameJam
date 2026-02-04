using DG.Tweening;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask : MonoBehaviour
{
    public MaskSO MaskSO;
    public SpriteRenderer SpriteRenderer;
    public LayerMask PlayerLayer;
    public int EntityId;
    public string Name;
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer.sprite = MaskSO.Sprite;
        Name = MaskSO.MaskName;
    }

    public virtual void BeUsed(Player Player)//��ʹ�õĶ���Ч��
    {
        //Player.GetComponentInParent<Player>();
        if(Player.IsLocalPlayer)
        {
            MsgSwapPositionRequest msg = new MsgSwapPositionRequest();
            msg.TargetPlayerId = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<BattleContext>().Players.Find(p => !p.IsLocalPlayer).playerId;
            GameEntry.Instance.GetSystem<NetSystem>().Send(msg);
        }
       
        transform.DOMove(Player.transform.position, 0.5f).OnComplete(()=>
        {
            Destroy(gameObject);
        });
    }
}
