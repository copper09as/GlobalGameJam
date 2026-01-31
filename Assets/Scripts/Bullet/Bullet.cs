using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;
    public Player Owner;

    private Vector3 moveDir;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    public void Init(Player owner,Vector3 initPosition, Vector3 dir)
    {
        Owner = owner;
        transform.position = initPosition;
        moveDir = dir.normalized;   // 再保险一次
    }
    void Update()
    {
        transform.position += moveDir * speed * Time.deltaTime;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
            Player hitPlayer = collision.gameObject.GetComponent<Player>();
            if(hitPlayer != Owner)
            {
                hitPlayer.Hp.Value -= 1;
                 Destroy(gameObject);
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


