using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mask", menuName = "ScriptableObjects/Mask")]
public class MaskSO : ScriptableObject
{
    public int Id;
    public string MaskName;
    public Sprite Sprite;
}
