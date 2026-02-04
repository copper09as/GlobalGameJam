using System.Collections.Generic;
using GameFramework;

public class BattleContext : GameContext
{
    public Player LocalPlayer;
    public Player SyncPlayer;
    public List<Bullet> Bullets = new ();
    public List<Wall> Walls = new ();

    protected override void OnInitialize()
    {
        
    }
    protected override void OnDispose()
    {
        
    }
}