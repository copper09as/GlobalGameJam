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
        owner.Rb.velocity = Vector2.zero;
    }

}

[StateBinding(typeof(Player), PlayerState.Idle)]
public class IdleState : StateBase<Player>
{
    public override void OnEnter(Player owner)
    {
        owner.Animator.Play("Idle");
    }
    public override void OnExit(Player owner)
    {
    }

}
[StateBinding(typeof(Player), PlayerState.Move)]
public class MoveState : StateBase<Player>
{
    public float Time = 1f;
    public float Timer = 0f;
    public override void OnEnter(Player owner)
    {
        UnityEngine.Debug.Log("Enter Move State");
        owner.Animator.Play("Walk");

        
    }
    public override void OnUpdate(Player owner, float deltaTime)
    {
        if (Timer <= 0.1f)
        {
            GameEntry.Instance.GetSystem<AudioSystem>().PlaySFXByName("�Ų�02");
            Timer = Time;
        }
        else
        {
            Time -= deltaTime;
        }
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