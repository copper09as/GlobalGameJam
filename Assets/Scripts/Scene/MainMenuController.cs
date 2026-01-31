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
        Debug.Log(ipInputField.inputText.text);
        Debug.Log(portInputField.inputText.text);
        if(!int.TryParse(portInputField.inputText.text, out int port))
        {
            GameEntry.Instance.GetSystem<GlobalUiSystem>().ShowNotification("input error","please input correct port number");
            return;
        }
        if(string.IsNullOrEmpty(ipInputField.inputText.text))
        {
            GameEntry.Instance.GetSystem<GlobalUiSystem>().ShowNotification("input error","please input correct ip address");
            return;
        }
        SceneManager.LoadScene("MainGame");
        
       

    }
}
