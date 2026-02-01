using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using Michsky.MUIP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]private CustomInputField ipInputField;
    [SerializeField]private CustomInputField portInputField;


    void Awake()
    {
        
        NetManager.AddEventListener(NetEvent.ConnectSucc,OnConnectSucc);
        NetManager.AddEventListener(NetEvent.ConnectFail,OnConnectFail);
        
    }
    void Start()
    {
        if(GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>()!=null)
        {
            GameEntry.Instance.GetSystem<ContextSystem>().DisposeContext<SessionContext>();
        }
        GameEntry.Instance.GetSystem<ContextSystem>().CreateContext<SessionContext>();
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().PlayerName = "Player"+(DateTime.Now.Ticks*31279%10000).ToString();
    }
    void OnDestroy()
    {
        NetManager.RemoveEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.RemoveEventListener(NetEvent.ConnectFail, OnConnectFail);
    }
    private void OnConnectFail(string err)
    {
        GameEntry.Instance.GetSystem<GlobalUiSystem>().ShowNotification("connect fail", err);
    }

    private void OnConnectSucc(string err)
    {
        GameEntry.Instance.GetSystem<GlobalUiSystem>().ShowNotification("connect success","successfully connected to server");
        SceneManager.LoadScene("MainGameScene");
    }

    public void ShowSettingPanel()
    {
        GameEntry.Instance.GetSystem<GlobalUiSystem>().ToggleGlobalSettingPanel();
    }
    public void EnterGame(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    public void ConnectToServer()
    {
        SceneManager.LoadScene("MainGame");
    }
    public void Exit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
