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
        //攻击影子，太阳
        
        collision.gameObject.GetComponent<IBeAttacked>()?.OnBeAttacked(this, moveDir);
        
        Debug.Log("触发事件");
    }

    public interface IBeAttacked
    {
        void OnBeAttacked(Bullet bullet,Vector3 moveDir);//前面是子弹，后面是攻击方向
    }
}


