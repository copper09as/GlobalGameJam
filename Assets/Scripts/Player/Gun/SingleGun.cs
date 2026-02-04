using UnityEngine;

[CreateAssetMenu(menuName = "Gun/SingleGun")]
public class SingleGun : Gun
{
    public override void Fire(Player owner, Vector3 targetPos)
    {
        CreateBullet(owner, targetPos);
    }

    protected void CreateBullet(Player owner, Vector3 targetPos)
    {
        var bulletObj = Instantiate(bulletPrefab, owner.FirePoint.position, Quaternion.identity);
        var bullet = bulletObj.GetComponent<Bullet>();
        bullet.Init(owner, owner.FirePoint.position, targetPos, bulletSpeed);
    }
}