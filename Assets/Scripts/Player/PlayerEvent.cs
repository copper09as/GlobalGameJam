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
    public struct PlayerMaskChange
    {
        public string id;
        public string MaskName;
    }
    public struct UseMaskEffect
    {
        public string id;
        public string MaskName;
    }

}