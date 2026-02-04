using GameFramework;
using JetBrains.Annotations;
using PlayerEvent;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Bullet;
public enum PlayerState
{
    Idle = 0,
    Move = 1,
    Attack = 2,
    Reload = 3
}
public class Player : GameStateMachineBehaviour<PlayerState, Player>, IBeAttacked
{
    //public float coldDownTime = 0.2f;
    public float currentColdDownTime = 10f;
    public bool InReload = false;
    public PlayerController controller;
    public Rigidbody2D Rb;
    public string playerId;
    public Transform FirePoint;
    public bool IsLocalPlayer = false;
    public bool StartGame = false;
    public string currentMaskName;
    public Animator Animator;
    public Gun Gun;

    public SpriteRenderer ShadowSR;//阴影渲染器
    public float beAttackTimer;//受击计时器
    public float beAttackDuration = 0.2f;//受击持续时间
    public Color normalColor = new Color(0, 0, 0, 0.5f);
    public Color beAttackedColor = new Color(1, 0, 0, 0.5f);
    // Start is called before the first frame update
    private AbilitySystemComponent abilitySystem;
    public AbilitySystemComponent AbilitySystem
    {
        get
        {
            if (abilitySystem == null)
            {
                InitializeAbilitySystem();
            }
            return abilitySystem;
        }
    }

    private void InitializeAbilitySystem()
    {
        abilitySystem = new AbilitySystemComponent();
        abilitySystem.InitializeFromDictionary(new Dictionary<string, float>
        {
            { "Hp", 10f},
            { "BulletCount", Gun.bulletCount},
            {"ReloadTimeRate", Gun.reloadTime},
            {"MoveSpeed", 5f},
            {"FireRate", Gun.fireCooldown}
        });
        abilitySystem.GetAttribute("Hp").WatchImmediate((value) => OnHpChanged(value));
        abilitySystem.GetAttribute("BulletCount").WatchImmediate((value) => OnBulletCountChanged(value));
        abilitySystem.GetAttribute("Hp").AddModifier(new AttributeModifier
        ("Hp", ModifierOp.Add,-5f));
    }

    private void OnBulletCountChanged(float value)
    {
        GameEntry.Instance.GetSystem<EventSystem>().Publish(new PlayerEvent.PlayerBulletChange
        {
            bulletCount = (int)value,
            MaxBulletCount = (int)AbilitySystem.GetAttribute("BulletCount").BaseValue,
            isLocalPlayer = IsLocalPlayer
        });
    }
    private void OnHpChanged(float newHp)
{
    GameEntry.Instance.GetSystem<EventSystem>().Publish(new PlayerEvent.PlayerHpChange
    {
        MaxHp = (int)abilitySystem.GetAttribute("Hp").BaseValue,
        hp = (int)newHp,
        isLocalPlayer = IsLocalPlayer
    });
    
}
    protected override void Start()
    {
        base.Start();
        Rb.gravityScale = 0f;
    }
    public float ReloadTime() => AbilitySystem.GetAttribute("ReloadTimeRate").Value*
    (abilitySystem.GetAttribute("ReloadTimeRate").BaseValue- abilitySystem.GetAttribute("ReloadTimeRate").Value + 1);

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!StartGame) return;
        if (controller != null)
            controller.Tick(this, Time.deltaTime);
        ShadowUpdate(Time.deltaTime);
    }
    public void Fire(Vector3 targetPos)
    {
        if (Gun == null) return;
        Gun.Fire(this, targetPos);
        Debug.Log("Fire");
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
        
        AbilitySystem.GetAttribute("BulletCount").ClearModifiers();
        InReload = false;
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
        if (collision.gameObject.CompareTag("Mask"))
        {
                MsgTakeMask msgTakeMask = new MsgTakeMask();
                msgTakeMask.EntityId = collision.gameObject.GetComponent<Mask>().EntityId;
                msgTakeMask.PlayerId = this.playerId;
                Debug.Log($"Player {msgTakeMask.PlayerId} is taking mask {msgTakeMask.EntityId}");
                GameEntry.Instance.GetSystem<NetSystem>().Send(msgTakeMask);
                //mask.BeUsed(this.gameObject);
                //UseMask(mask.Name);
        }
        //if (collision.gameObject.CompareTag("Bullet"))
        //{
        //    var bullet = collision.gameObject.GetComponentInParent<Bullet>();
        //    OnBeAttacked(bullet, Vector3.zero, Vector3.zero);
        //    Debug.Log("Plyer里面触发了trigger");
        //}
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
        StateMachine.RegisterState(PlayerState.Move, new MoveState());
        StateMachine.RegisterState(PlayerState.Idle, new IdleState());
    }

    public void OnBeAttacked(Bullet bullet, Vector3 moveDir, Vector3 hit)
    {
        if (bullet.Owner == this) return;
        //受击反馈
        beAttackTimer = beAttackDuration;
        GameEntry.Instance.GetSystem<AudioSystem>().PlaySFXByName("角色受击");
        if (IsLocalPlayer)
        {
            abilitySystem.GetAttribute("Hp").AddModifier(new AttributeModifier
            ("Hp", ModifierOp.Add, -1f));
            
        }
    }
    public void ShadowUpdate(float deltaTime)
    {
        if (beAttackTimer >= 0.1f)
        {
            beAttackTimer -= deltaTime;
            ShadowSR.color = beAttackedColor;
        }
        else
        {
            ShadowSR.color = normalColor;
        }
    }
}
