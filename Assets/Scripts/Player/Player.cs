using GameFramework;
using JetBrains.Annotations;
using PlayerEvent;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;
public enum PlayerState
{
    Idle,
    Move,
    Attack,
    Reload,
    Die
}
public class Player : GameStateMachineBehaviour<PlayerState, Player>, IBeAttacked
{   
    public float coldDownTime = 0.2f;
    public float currentColdDownTime = 0f;
    public bool InReload = false;
    public ScPlayerController controller;
    public Rigidbody2D Rb;
    [SerializeField]protected float reloadTimeRate = 1;
    public string playerName;
    public float moveSpeed = 5f;
    public int MaxBullet = 10;
    public ReactiveInt BulletCount = new ReactiveInt(10);
    public Transform FirePoint;
    public int MaxHp = 10;
    public ReactiveInt Hp = new ReactiveInt(10);

    public bool startGame   = false;
    public Vector2 MoveDirection;

    public Animator Animator;

    public SpriteRenderer ShadowSR;//阴影渲染器
    public float beAttackTimer;//受击计时器
    public float beAttackDuration = 0.2f;//受击持续时间
    public Color normalColor = new Color(0,0,0,0.5f);
    public Color beAttackedColor = new Color(1,0,0,0.5f);
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Rb.gravityScale = 0f;
        Debug.Log($"Player {playerName} started with {BulletCount.Value} bullets and {Hp.Value} HP.");
    }
    

    public float ReloadTime()=> (MaxBullet-BulletCount.Value) * reloadTimeRate;
    // Update is called once per frame
    protected override void Update()
    {
        base.Update(); 
        if(!startGame) return;
        controller.ControlMove(this);
        controller.Rotate(this);
        controller.Fire(this);
        controller.Reload(this);
        controller.Tick(this, Time.deltaTime);
        ShadowUpdate(Time.deltaTime);
    }
    public void CreateBullet(Vector3 targetPosition, Vector3 firePosition = default(Vector3),bool isSync = false)
    {
        GameObject bulletObj = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet"));
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if(FirePoint!=null)
        {
            bullet.Init(this,FirePoint.transform.position, targetPosition,isSync);
            return;
        }
        bullet.Init(this, firePosition, targetPosition,isSync);
    }
    public void StartReload()
    {
        if (InReload) return;

        InReload = true;
        reloadCoroutine = StartCoroutine(ReloadCoroutine());
    }
    private Coroutine reloadCoroutine;
    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(ReloadTime());
        BulletCount.Value = MaxBullet;
        GameEntry.Instance.GetSystem<EventSystem>().Publish(new PlayerEvent.PlayerBulletChange {
            CurrentBullet = BulletCount.Value,
            MaxBullet = MaxBullet,
            id = playerName
        });
        InReload = false;
        //StateMachine.ChangeState(PlayerState.Idle);
    }
    public void StopReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }

        InReload = false;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!startGame) return;
        if (collision.gameObject.CompareTag("Mask"))
        {
            Mask mask = collision.gameObject.GetComponent<Mask>();
            mask.BeUsed(this.gameObject);
            UseMask(mask.Name);
        }
    }
    protected override Player GetOwner()
    {
        return this;
    }
    protected override PlayerState GetInitialState()
    {
        return PlayerState.Idle; 
    }
    protected override void ConfigureStateMachine()
    {
        StateMachine.RegisterState(PlayerState.Reload, new ReloadState());
        StateMachine.RegisterState(PlayerState.Move, new MoveState());
        StateMachine.RegisterState(PlayerState.Idle, new IdleState());
        StateMachine.RegisterState(PlayerState.Attack, new AttackState());

        StateMachine.AddTransition
        (PlayerState.Idle, PlayerState.Move, (i) => MoveDirection.magnitude > 0.1f);
        StateMachine.AddTransition
        (PlayerState.Move, PlayerState.Idle, (i) => MoveDirection.magnitude <= 0.1f);
    }
  

    public void UseMask(string maskName)
    {
        //触发对应面具的效果
        //获取面具
        //切换脸上面具
        GameEntry.Instance.GetSystem<EventSystem>().Publish(new PlayerEvent.UseMaskEffect
        {
            id = playerName,
            MaskName = maskName
        });

    }

    public void OnBeAttacked(Bullet bullet, Vector3 moveDir, Vector3 hit)
    {
        if (bullet.Owner.playerName == playerName) return;
        Hp.Value -= 1;
        GameEntry.Instance.GetSystem<EventSystem>().Publish(new PlayerEvent.PlayerHpChange
        {
            hp = Hp.Value,
            MaxHp = MaxHp,
            id = playerName
        });

        //受击反馈
        beAttackTimer = beAttackDuration;

        Destroy(bullet.gameObject);
        if (Hp.Value <= 0)
        {
            Debug.Log($"Player {playerName} died.");
        }
    }

    public void ShadowUpdate(float deltaTime)
    {
        if(beAttackTimer>=0.1f)
        {
            beAttackTimer-= deltaTime;
            ShadowSR.color = beAttackedColor;
        }
        else
        {
            ShadowSR.color = normalColor;
        }
    }
}
