using UnityEngine;

[CreateAssetMenu(menuName = "Gun/TripleSpreadGun")]
public class TripleSpreadGun : Gun
{
     public float spreadAngle = 0f;      // 散射角（给三散射用）
    public override void Fire(Player owner, Vector3 targetPos)
    {
        Vector3 dir = (targetPos - owner.FirePoint.position).normalized;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        FireWithAngle(owner, baseAngle);
        FireWithAngle(owner, baseAngle + spreadAngle);
        FireWithAngle(owner, baseAngle - spreadAngle);
    }

    private void FireWithAngle(Player owner, float angle)
    {
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        Vector3 dir = rot * Vector3.right;

        var bulletObj = Instantiate(bulletPrefab, owner.FirePoint.position, Quaternion.identity);
        var bullet = bulletObj.GetComponent<Bullet>();
        bullet.Init(owner, owner.FirePoint.position, owner.FirePoint.position + dir, bulletSpeed);
    }
}
