using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask : MonoBehaviour
{
    public MaskSO MaskSO;
    public SpriteRenderer SpriteRenderer;
    public LayerMask PlayerLayer;
    public string Name;
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer.sprite = MaskSO.Sprite;
        Name = MaskSO.MaskName;
    }

    public virtual void BeUsed(GameObject Player)//��ʹ�õĶ���Ч��
    {
        //Player.GetComponentInParent<Player>();
        transform.DOMove(Player.transform.position, 0.5f).OnComplete(()=>
        {
            Destroy(gameObject);
        });
    }
}
