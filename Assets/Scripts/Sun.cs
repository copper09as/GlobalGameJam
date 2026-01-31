using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using static Bullet;

public class Sun : MonoBehaviour,IBeAttacked
{
    private static Sun instance;
    public float speed = 10f;//̫����ǰ�����ٶ�
    public float normalSpeed = 10f;//̫�������ٶ�
    public float shakedSpeed = 50f;//��ʱ̫���ٶ�
    public Sprite NormalSun;//ƽ����̫��ͼƬ
    public Sprite ShakedSun;//������ʱ��̫��ͼƬ
    public Sprite DarkSun;//�ڰ�̫��ͼƬ
    public SpriteRenderer sr;//̫��ͼƬ��Ⱦ��
    private bool isStart = false;//̫���Ƿ�ʼ����

    [SerializeField]
    private SolarOrbit Orbit;//���

    private int direction = 1;//���з��� 1Ϊ��ʱ�� -1Ϊ˳ʱ��

    private int target = 0;//Ŀ���
    [SerializeField]
    private float shakeDuration = 0.5f;//�𶯳���ʱ��
    [SerializeField]
    private float shakeMagnitude = 0.1f;//�𶯷���
    [SerializeField]
    private float shaketime = 0f;//�𶯼�ʱ
    public static Sun Instance//ȫ�ֵ���
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
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
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
        //    Debug.Log("��ʼ��");
        //    shaketime = shakeDuration;
            
        //}
        if(isStart) Playing(Time.deltaTime);
        Shake(Time.deltaTime);
        
    }
    public void SunStart()
    {
        isStart = true;
    }

    public void Playing(float deltaTime)//̫���ڹ����������
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

            sr.sprite = ShakedSun;//�л�̫��ͼƬ
        }
        else
        {
            speed = normalSpeed;
            sr.sprite = NormalSun;//�л�̫��ͼƬ
        }
    }
    public void SwitchDirection()//�л�����
    {
        direction = - direction;
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    //��ײ�¼���̫���ܻ�
    //    if (collision.gameObject.CompareTag("Bullet"))
    //    {
    //        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
    //        Vector3 moveDir = bullet.transform.position - transform.position;
    //        OnBeAttacked(bullet, moveDir);
    //    }
    //}
   

    public void OnBeAttacked(Bullet bullet, Vector3 moveDir, Vector3 hit)
    {
        Debug.Log("��ʼ��");
        shaketime = shakeDuration;
        //���ݹ����ķ���
        var a = Vector2.Dot(transform.position-hit, GetDirection(Orbit.points[target]));
        if (a < 0f) SwitchDirection();//�ı䷽��
    }
}
