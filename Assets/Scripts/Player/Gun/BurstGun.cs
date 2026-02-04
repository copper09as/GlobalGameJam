using System.Collections;
using System.Collections.Generic;   
using UnityEngine;
[CreateAssetMenu(menuName = "Gun/BurstGun")]
public class BurstGun : Gun
{
    public int burstCount = 1;          // 连发次数（给三连发用）
    public float burstInterval = 0.1f;  // 连发间隔时间
    public override void Fire(Player owner, Vector3 targetPos)
    {
        owner.StartCoroutine(BurstFire(owner, targetPos));
    }

    private IEnumerator BurstFire(Player owner, Vector3 targetPos)
    {
        for (int i = 0; i < burstCount; i++)
        {
            var bulletObj = Instantiate(bulletPrefab, owner.FirePoint.position, Quaternion.identity);
            var bullet = bulletObj.GetComponent<Bullet>();
            bullet.Init(owner, owner.FirePoint.position, targetPos, bulletSpeed);

            yield return new WaitForSeconds(burstInterval);
        }
    }
}
