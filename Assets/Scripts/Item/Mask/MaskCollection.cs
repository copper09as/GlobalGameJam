using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MaskCollection", menuName = "ScriptableObjects/MaskCollection")]
public class MaskCollection : ScriptableObject
{
    public List<MaskSO> MaskDataList;
}
