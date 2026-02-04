using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GunCollection", menuName = "ScriptableObjects/GunCollection")]
public class GunCollection: ScriptableObject
{
    public List<Gun> Guns;

    public Gun GetGunByName(string gunName)
    {
        foreach (var gun in Guns)
        {
            if (gun.GunName == gunName)
            {
                return gun;
            }
        }
        return null;
    }
    public Gun GetGunById(int id)
    {
        foreach (var gun in Guns)
        {
            if (gun.Id == id)
            {
                return gun;
            }
        }
        return null;
    }
}