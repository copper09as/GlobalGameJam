using DG.Tweening;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Mask : MonoBehaviour
{
    private MaskSO MaskSO;
    [SerializeField]private SpriteRenderer SpriteRenderer;
    public LayerMask PlayerLayer;
    public int EntityId;
    public string Name;
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();

    }
    public void Init(MaskSO maskSO)
    {
        this.MaskSO = maskSO;
        SpriteRenderer.sprite = maskSO.Sprite;
        Name = maskSO.MaskName;
        this.name = maskSO.MaskName;
    }

    public virtual void BeUsed(Player Player)//��ʹ�õĶ���Ч��
    {
        //Player.GetComponentInParent<Player>();
        if(Player.IsLocalPlayer)
        {
            switch(MaskSO.Id)
            {
                case 0:
                    MsgSwapPositionRequest msg = new MsgSwapPositionRequest();
                    msg.TargetPlayerId = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<BattleContext>().Players.Find(p => !p.IsLocalPlayer).playerId;
                    GameEntry.Instance.GetSystem<NetSystem>().Send(msg);
                    break;
                case 1:
                    MsgShineEffect msgShine = new MsgShineEffect();
                    GameEntry.Instance.GetSystem<NetSystem>().Send(msgShine);
                    break;
                case 2:
                    if(GameEntry.Instance.GetSystem<ContextSystem>().GetContext<BattleContext>().LocalPlayer.AbilitySystem.GetAttribute("Hp").Value>=10)
                    {
                        return;
                    }
                    GameEntry.Instance.GetSystem<ContextSystem>().GetContext<BattleContext>().LocalPlayer.
                    AbilitySystem.GetAttribute("Hp").AddModifier
                    (new AttributeModifier("Hp", ModifierOp.Add, 1));
                    break;
            }

        }
       
        transform.DOMove(Player.transform.position, 0.5f).OnComplete(()=>
        {
            Destroy(gameObject);
        });
    }
}
