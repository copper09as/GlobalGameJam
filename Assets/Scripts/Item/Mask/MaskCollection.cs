using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MaskCollection", menuName = "ScriptableObjects/MaskCollection")]
public class MaskCollection : ScriptableObject
{
    public List<MaskSO> MaskDataList;

    public MaskSO GetMaskSOByName(string maskName)
    {
        foreach (var mask in MaskDataList)
        {
            if (mask.MaskName == maskName)
            {
                return mask;
            }
        }
        return null;
    }
}
