using DG.Tweening;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            switch(MaskSO.Id)
            {
                case 1:
                    MsgSwapPositionRequest msg = new MsgSwapPositionRequest();
                    msg.TargetPlayerId = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<BattleContext>().Players.Find(p => !p.IsLocalPlayer).playerId;
                    GameEntry.Instance.GetSystem<NetSystem>().Send(msg);
                    break;
                case 2:
                    MsgShineEffect msgShine = new MsgShineEffect();
                    GameEntry.Instance.GetSystem<NetSystem>().Send(msgShine);
                    break;
                case 3:
                    GameEntry.Instance.GetSystem<AudioSystem>().PlaySFXByName("面具加速01");
                    break;
            }

        }
       
        transform.DOMove(Player.transform.position, 0.5f).OnComplete(()=>
        {
            Destroy(gameObject);
        });
    }
}
