namespace PlayerEvent
{
    public struct PlayerHpChange
    {
        public string id;
        public int hp;
        public int MaxHp;
    }
    public struct PlayerBulletChange
    {
        public string id;
        public int CurrentBullet;
        public int MaxBullet;
    }
}