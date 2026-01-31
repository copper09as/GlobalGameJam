using UnityEngine;

public class ScPlayerController:ScriptableObject
{
    
    public virtual void ControlMove(Player player)
    {

    }
    public virtual void SetPosition(Player player,Vector2 position)
    {

    }
    public virtual void Update(Player player, float deltaTime)
    {

    }
    public virtual void Rotate(Player player)
    {

    }
    public virtual void Reload(Player player)
    {
        
    }
    public virtual void Fire(Player player,Vector2 targetPosition,Vector2 firePosition)
    {
        player.CreateBullet(targetPosition, firePosition);
    }
    public virtual void Rotate(Player player, float angle)
    {
    }
    public virtual void Fire(Player player)
    {
    }
}



