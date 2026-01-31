using System.Diagnostics;
using GameFramework;
using UnityEngine;
[StateBinding(typeof(Player), PlayerState.Reload)]
public class ReloadState : StateBase<Player>
{
    public override void OnEnter(Player owner)
    {
        
    }
    public override void OnExit(Player owner)
    {
    }

}

[StateBinding(typeof(Player), PlayerState.Idle)]
public class IdleState : StateBase<Player>
{
    public override void OnEnter(Player owner)
    {
        UnityEngine.Debug.Log("Enter Idle State");
    }
    public override void OnUpdate(Player owner, float deltaTime)
    {
    }
    public override void OnExit(Player owner)
    {
    }

}
[StateBinding(typeof(Player), PlayerState.Move)]
public class MoveState : StateBase<Player>
{
    public override void OnEnter(Player owner)
    {
        UnityEngine.Debug.Log("Enter Move State");
    }
    public override void OnUpdate(Player owner, float deltaTime)
    {
        Vector2 moveDir = owner.MoveDirection.normalized;
        // 直接设置位置而不是速度，避免物理推动
        owner.transform.position += (Vector3)moveDir * owner.moveSpeed * deltaTime;
        owner.Rb.velocity = Vector2.zero; // 清除物理速度，防止被其他物体推动
    }
    public override void OnExit(Player owner)
    {
        owner.Rb.velocity = Vector2.zero;
    }

}
[StateBinding(typeof(Player), PlayerState.Attack)]
public class AttackState : StateBase<Player>
{
    public override void OnEnter(Player owner)
    {
    }
    public override void OnUpdate(Player owner, float deltaTime)
    {
    }
    public override void OnExit(Player owner)
    {
    }

}