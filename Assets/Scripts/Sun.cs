using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private bool isDark = false;

    [SerializeField] private float darkSpeed = 20f;   // 黑暗模式速度
    public Sprite NormalSun;
    public Sprite ShakedSun;
    public Sprite DarkSun;
    public SpriteRenderer sr;
    public bool StartGame = false;

    private float Darktimer = 0.1f;
    [SerializeField]
    private SolarOrbit Orbit;//���

    public int direction = 1;//���з��� 1Ϊ��ʱ�� -1Ϊ˳ʱ��

    public int target = 0;//Ŀ���
    [SerializeField]
    private float shaketime = 0f;//�𶯼�ʱ
    public Vector3 GetDirection(Transform entity)
    {
        return entity.position - transform.position;
    }
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<BattleContext>().Sun = this;
    }
    void Update()
    {
        if(StartGame) MoveToNextPoint(Time.deltaTime);

    }
    public void MoveToNextPoint(float deltaTime)
    {
        MoveToPoint(target, deltaTime);
        if (Vector3.Distance(transform.position, Orbit.points[target].position) < 0.1f)
        {
            target += direction;
            if (target >= Orbit.points.Length)
            {
                target = 0;
            }
            else if (target < 0)
            {
                target = Orbit.points.Length - 1;
            }
        }
    }
    public void MoveToPoint(int pointIndex, float deltaTime)
    {
        transform.position = Vector3.MoveTowards(transform.position, Orbit.points[pointIndex].position, deltaTime * speed);
    }
    public void OnBeAttacked(Bullet bullet, Vector3 moveDir, Vector3 hit)
    {
        if (isDark) return;   // 黑暗模式免疫攻击
        var a = Vector2.Dot(transform.position-hit, GetDirection(Orbit.points[target]));
        if (a < 0f) 
        {
            direction = -direction;
            SyncSunToServer();
        }
        
    }
    public void SyncSunToServer()
    {
        MsgSyncSun msg = new MsgSyncSun
        {
            posX = transform.position.x,
            posY = transform.position.y,
            target = target,
            direction = direction
        };
        GameEntry.Instance.GetSystem<NetSystem>().Send(msg);
    }
}
