using System;
using System.Collections.Generic;
using GameFramework;

public class BattleContext : GameContext
{
    public List<Player> Players = new ();
    public Player LocalPlayer=>Players.Find(p=>p.IsLocalPlayer);
    public Player SyncPlayer=>Players.Find(p=>!p.IsLocalPlayer);
    public List<Bullet> Bullets = new ();
    public List<Wall> Walls = new ();
    public List<Mask> Masks = new ();
    public Sun Sun;

    protected override void OnInitialize()
    {
        
    }
    protected override void OnDispose()
    {
        
    }

    internal void StartBattle()
    {
        LocalPlayer.StartGame = true;
        SyncPlayer.StartGame = true;
        Sun.StartGame = true;
    }
}