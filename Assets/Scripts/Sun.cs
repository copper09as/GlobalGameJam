using System.Collections;
using System.Collections.Generic;
using System.Xml;
using GameFramework;
using Unity.VisualScripting;
using UnityEngine;
using static Bullet;

public class Sun : MonoBehaviour,IBeAttacked
{
    private static Sun instance;
    public float speed = 10f;
    public float normalSpeed = 10f;
    public float shakedSpeed = 50f;
    public Sprite NormalSun;
    public Sprite ShakedSun;
    public Sprite DarkSun;
    public SpriteRenderer sr;
    private bool isStart = false;

    private float Darktimer = 0f;
    [SerializeField]
    private SolarOrbit Orbit;//���

    private int direction = 1;//���з��� 1Ϊ��ʱ�� -1Ϊ˳ʱ��

    private int target = 0;//Ŀ���
    [SerializeField]
    private float shakeDuration = 1f;//�𶯳���ʱ��
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
        if(Darktimer <= 0.1f)
        {
            Shake(Time.deltaTime);
        }
        else
        {
            Darktimer -= Time.deltaTime;
            sr.sprite = DarkSun;
        }
        if(GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer==null||
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer==null)
        {
            return;
        }
        if(GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>
        ().LocalPlayer.currentMaskName == "EvilMask"||GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>
        ().SyncPlayer.currentMaskName == "EvilMask")
        {
             Darktimer = 5f;
             if(GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.currentMaskName == "EvilMask")
             {
                 GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().LocalPlayer.currentMaskName = "";
             }
             if (GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer.currentMaskName == "EvilMask")
             {
                 GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().SyncPlayer.currentMaskName = "";
             }
        }
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
            if (shaketime > 0.5f)
            {
                transform.localPosition += Random.insideUnitSphere * shakeMagnitude;
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);

                speed = shakedSpeed;
            }
            sr.sprite = ShakedSun;
            Debug.Log("Shaking");
        }
        else
        {
            speed = normalSpeed;
            sr.sprite = NormalSun;
        }
    }
    public void SwitchDirection()
    {
        direction = - direction;
    }
    public void OnBeAttacked(Bullet bullet, Vector3 moveDir, Vector3 hit)
    {
        shaketime = shakeDuration;
        var a = Vector2.Dot(transform.position-hit, GetDirection(Orbit.points[target]));
        if (a < 0f) SwitchDirection();//�ı䷽��
    }

    /// <summary>
    /// 更换黑暗太阳精灵并且持续time秒
    /// </summary>
    /// <param name="time"></param>
    public void SwitchDarkSun(float time)
    {
        Darktimer = time;
    }
}
