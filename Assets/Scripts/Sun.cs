using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using static Bullet;

public class Sun : MonoBehaviour,IBeAttacked
{
    private static Sun instance;
    public float speed = 10f;//太阳当前运行速度
    public float normalSpeed = 10f;//太阳正常速度
    public float shakedSpeed = 50f;//震动时太阳速度
    public Sprite NormalSun;//平常的太阳图片
    public Sprite ShakedSun;//被攻击时的太阳图片
    public Sprite DarkSun;//黑暗太阳图片
    public SpriteRenderer sr;//太阳图片渲染器

    [SerializeField]
    private SolarOrbit Orbit;//轨道

    private int direction = 1;//运行方向 1为逆时针 -1为顺时针

    private int target = 0;//目标点
    [SerializeField]
    private float shakeDuration = 0.5f;//震动持续时间
    [SerializeField]
    private float shakeMagnitude = 0.1f;//震动幅度
    [SerializeField]
    private float shaketime = 0f;//震动计时
    public static Sun Instance//全局单例
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    public Vector3 GetDirection(Transform entity)
    {
        return entity.position - transform.position;
    }

    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        sr = GetComponent<SpriteRenderer>();
        Random.InitState(System.DateTime.Now.Millisecond);
    }
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("开始震动");
        //    shaketime = shakeDuration;
            
        //}
        Playing(Time.deltaTime);
        Shake(Time.deltaTime);
        
    }
    public void Playing(float deltaTime)//太阳在轨道上面运行
    {
        transform.position = Vector3.MoveTowards(transform.position, Orbit.points[target].position, deltaTime * speed);
        if (Vector3.Distance(transform.position, Orbit.points[target].position) < 0.1f)
        {
            target += direction;
            if (target >= Orbit.points.Length)
            {
                target = 0;
            }
            if (target == -1)
            {
                target = Orbit.points.Length - 1;
            }
        }
    }

    public void Shake(float deltaTime)
    {
        if(shaketime > 0f)
        {
            shaketime -= deltaTime;

            transform.localPosition += Random.insideUnitSphere * shakeMagnitude;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);

            speed = shakedSpeed;

            sr.sprite = ShakedSun;//切换太阳图片
        }
        else
        {
            speed = normalSpeed;
            sr.sprite = NormalSun;//切换太阳图片
        }
    }
    public void SwitchDirection()//切换方向
    {
        direction = - direction;
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    //碰撞事件，太阳受击
    //    if (collision.gameObject.CompareTag("Bullet"))
    //    {
    //        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
    //        Vector3 moveDir = bullet.transform.position - transform.position;
    //        OnBeAttacked(bullet, moveDir);
    //    }
    //}
   

    public void OnBeAttacked(Bullet bullet, Vector3 moveDir, Vector3 hit)
    {
        Debug.Log("开始震动");
        shaketime = shakeDuration;
        //根据攻击的方向
        var a = Vector2.Dot(transform.position-hit, GetDirection(Orbit.points[target]));
        if (a < 0f) SwitchDirection();//改变方向
    }
}
