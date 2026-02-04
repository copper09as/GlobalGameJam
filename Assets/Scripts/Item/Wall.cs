using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class Wall : MonoBehaviour, IBeAttacked
{

    private int hp = 3;
    [SerializeField] int maxHP = 3;
    public SpriteRenderer spriteRenderer;
    public Sprite DamagedWall;
    private bool isDestroyed = false;
    private BattleContext battleContext
    {
        get
        {
            return GameEntry.Instance.GetSystem<ContextSystem>().GetContext<BattleContext>();
        }
    }
    public int HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp <= maxHP/ 2)
            {
                spriteRenderer.sprite = DamagedWall;
                
            }
        }
    }
    public void OnBeAttacked(Bullet bullet, Vector3 moveDir, Vector3 hit)
    {
        HP--;
        if (HP<=0)
        {
            BeDestroyed();
        }
        GameEntry.Instance.GetSystem<AudioSystem>().PlaySFXByName("木墙受击01");
    }

    void Start()
    {
   
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    
    void BeDestroyed()
    {
        /*if(Random.Range(1,4) ==1 ) 
        {
            var collection = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().maskCollection;
            GreatMask(collection.MaskDataList[Random.Range(0, collection.MaskDataList.Count)]).transform.position = transform.position;
        }
        GameEntry.Instance.GetSystem<AudioSystem>().PlaySFXByName("木墙破碎01");*/
        if (isDestroyed) return;
        isDestroyed = true;
        MsgCreateMask msg = new MsgCreateMask();
        msg.posX = transform.position.x;
        msg.posY = transform.position.y;
        GameEntry.Instance.GetSystem<NetSystem>().Send(msg);
        Destroy(gameObject,0.2f);
    }

    private void OnDestroy()
    {
    }
       


}
