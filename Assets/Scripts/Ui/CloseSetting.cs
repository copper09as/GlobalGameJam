using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

public class CloseSetting : MonoBehaviour
{

    public void ClosePanel()
    {
        GlobalUiSystem globalUiSystem = GameEntry.Instance.GetSystem<GlobalUiSystem>();
        globalUiSystem.HideGlobalSettingPanel();
    }
}
