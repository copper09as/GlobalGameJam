using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    void Awake()
    {
        var contextSystem = GameEntry.Instance.GetSystem<ContextSystem>();
        if(contextSystem.GetContext<SessionContext>() == null)
        {
            contextSystem.DisposeContext<SessionContext>();
        }
        contextSystem.CreateContext<SessionContext>();
    }
    public void ShowSettingPanel()
    {
        GameEntry.Instance.GetSystem<GlobalUiSystem>().ToggleGlobalSettingPanel();
    }
}
