namespace PlayerEvent
{
    public struct PlayerHpChange
    {
        public bool isLocalPlayer;
        public int hp;
        public int MaxHp;
    }
    public struct PlayerBulletChange
    {
        public bool isLocalPlayer;
        public int bulletCount;
        public int MaxBulletCount;
    }

}