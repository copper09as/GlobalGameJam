using GameFramework;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 5f;
    public Player Owner;
    [SerializeField]private Rigidbody2D rb;
    private Vector3 moveDir;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    public void Init(Player owner, Vector3 initPosition, Vector3 targetPosition,float speed = 10f)
    {
        Owner = owner;
        transform.position = initPosition;

        Vector3 targetPos = targetPosition;
        targetPos.z = initPosition.z; // 保证同一平面
        Vector2 dir = (targetPos - initPosition).normalized;
        rb.velocity = dir * speed;

        var angle = Vector2.SignedAngle(Vector2.right, dir);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        GameEntry.Instance.GetSystem<AudioSystem>().PlaySFXByName("发射02");
    }

    //子弹只有Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var p = collision.gameObject.GetComponentInParent<Player>();
        //攻击太阳，阴影，
        if (p!=null)
        {
            if (p == Owner)
            {
                Debug.Log("子弹碰到自己，忽略");
                return;
            }
            else
            {
                if (!collision.isTrigger) //此时子弹先碰撞到对方本体
                { Destroy(gameObject); return; }
            }
        }
        var c = collision.gameObject.GetComponentInParent<IBeAttacked>();
        c?.OnBeAttacked(this, moveDir,transform.position);//子弹的体积小，中心点约等于碰撞点
        if(c!=null)Destroy(gameObject);
        UnityEngine.Debug.Log("子弹触发器碰撞到物体：" + collision.gameObject.name);
        return;
    }

    public interface IBeAttacked
    {
        void OnBeAttacked(Bullet bullet, Vector3 moveDir, Vector3 hit); // 移除默认参数
    }
}


