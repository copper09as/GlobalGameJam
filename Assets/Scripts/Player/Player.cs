using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
public enum PlayerState
{
    Idle,
    Move,
    Attack,
    Die
}
public class Player : GameStateMachineBehaviour<PlayerState, Player>
{   
    [SerializeField]protected Rigidbody2D rb;
    public string playerName;
        public float moveSpeed = 5f;
    public ReactiveInt BulletCount = new ReactiveInt(10);
    public ReactiveInt Hp = new ReactiveInt(10);
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Listen<PlayerEvent.PlayerHpChange>(evt =>
        {
            GameEntry.Instance.GetSystem<EventSystem>().Publish<PlayerEvent.PlayerHpChange>(evt);
        });
        Listen<PlayerEvent.PlayerBulletChange>(evt =>
        {
            GameEntry.Instance.GetSystem<EventSystem>().Publish<PlayerEvent.PlayerBulletChange>(evt);
        });
    }

    // Update is called once per frame
    protected override void Update()
    {
       base.Update(); 
    }
public void CreateBullet(Vector3 targetPosition)
{
    GameObject bulletObj = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet"));
    Bullet bullet = bulletObj.GetComponent<Bullet>();

    bullet.Init(this,transform.position, targetPosition);
}

    protected override Player GetOwner()
    {
        return this;
    }

    protected override PlayerState GetInitialState()
    {
        return PlayerState.Idle;
    }

}
