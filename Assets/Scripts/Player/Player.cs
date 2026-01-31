using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using JetBrains.Annotations;
using PlayerEvent;
using UnityEditor;
using UnityEngine;
public enum PlayerState
{
    Idle,
    Move,
    Attack,
    Reload,
    Die
}
public class Player : GameStateMachineBehaviour<PlayerState, Player>
{
    public ScPlayerController controller;
    public Rigidbody2D Rb;
    [SerializeField]protected float reloadTimeRate;
    public string playerName;
    public float moveSpeed = 5f;
    public int MaxBullet = 10;
    public ReactiveInt BulletCount = new ReactiveInt(10);
    public Transform FirePoint;
    public ReactiveInt Hp = new ReactiveInt(10);
    public Vector2 MoveDirection;
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
        controller.ControlMove(this);
        controller.Rotate(this);
        controller.Fire(this);
    }
    public void CreateBullet(Vector3 targetPosition, Vector3 firePosition = default(Vector3))
    {
        GameObject bulletObj = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet"));
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if(FirePoint!=null)
        {
            bullet.Init(this,FirePoint.transform.position, targetPosition);
            return;
        }
        bullet.Init(this, firePosition, targetPosition);
    }
    public void ReloadBullets()
    {
        ChangeState(PlayerState.Reload);
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

    public void TakeDamage(int damage)
    {
        Hp.Value -= damage;
        Debug.Log($"Player {playerName} took {damage} damage, remaining HP: {Hp.Value}");
        if (Hp.Value <= 0)
        {
            Debug.Log($"Player {playerName} has died.");
            // Handle player death (e.g., change state, play animation, etc.)
        }
    }

}
