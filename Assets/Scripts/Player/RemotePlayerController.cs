using UnityEngine;

[CreateAssetMenu(menuName ="Player/RemotePlayerController")]
public class RemotePlayerController : ScPlayerController
{
    public override void SetPosition(Player player,Vector2 position)
    {
        player.transform.position = position;
    }
    public override void Rotate(Player player, float angle)
    {
         player.FirePoint.transform.parent.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
