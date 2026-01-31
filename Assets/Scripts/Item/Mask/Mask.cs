using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask : MonoBehaviour
{
    public MaskSO MaskSO;
    public SpriteRenderer SpriteRenderer;
    public string Name;
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer.sprite = MaskSO.Sprite;
        Name = MaskSO.MaskName;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.GetMask("Player"))
        { 
            BeUsed(collision.gameObject);
        }
    }

    public virtual void BeUsed(GameObject Player)//被使用的动画效果
    {
        //Player.GetComponentInParent<Player>();
        transform.DOMove(Player.transform.position, 0.5f).OnComplete(()=>
        {
            Destroy(gameObject);
        });
    }
}
