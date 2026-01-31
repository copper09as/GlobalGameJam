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
}


