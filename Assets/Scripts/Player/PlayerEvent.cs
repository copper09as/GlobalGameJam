namespace PlayerEvent
{
    public struct PlayerHpChange
    {
        public string id;
        public int hp;
    }
    public struct PlayerBulletChange
    {
        public string id;
        public int bulletCount;
    }
}