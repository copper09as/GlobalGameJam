using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;

    private Vector3 moveDir;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Init(Vector3 initPosition, Vector3 dir)
    {
        transform.position = initPosition;
        moveDir = dir.normalized;   // 再保险一次
    }

    void Update()
    {
        transform.position += moveDir * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}


