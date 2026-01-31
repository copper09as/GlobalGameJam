using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;
    public Player Owner;
    public float syncSpeed;
    public float currentSpeed;
    [SerializeField]private Rigidbody2D rb;
    private Vector3 moveDir;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
public void Init(Player owner, Vector3 initPosition, Vector3 targetPosition, bool isSync = false)
{
    currentSpeed = isSync ? syncSpeed : speed;
    Owner = owner;
    transform.position = initPosition;

    Vector3 targetPos = targetPosition;
    targetPos.z = initPosition.z; // 保证同一平面
    Vector2 dir = (targetPos - initPosition).normalized;
    rb.velocity = dir * currentSpeed;

    var angle = Vector2.SignedAngle(Vector2.right, dir);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
            Player hitPlayer = collision.gameObject.GetComponent<Player>();
            if(hitPlayer != Owner)
            {
                hitPlayer.Hp.Value -= 1;
                Destroy(gameObject);
                return;
            }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //攻击太阳，影子
        var c = collision.gameObject.GetComponent<IBeAttacked>();
        c?.OnBeAttacked(this, moveDir,transform.position);//子弹的体积小，中心点约等于碰撞点
        if(c!=null)Destroy(gameObject);
    }

    public interface IBeAttacked
    {
        void OnBeAttacked(Bullet bullet, Vector3 moveDir, Vector3 hit); // 移除默认参数
    }
}


