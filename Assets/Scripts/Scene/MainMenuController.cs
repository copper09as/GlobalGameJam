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

    [SerializeField] private RoomPanel roomPanel;
    [SerializeField] private RoomListView roomListView;
    private NetSystem netSystem;
    private SessionContext sessionContext;

    void Awake()
    {
        netSystem = GameEntry.Instance.GetSystem<NetSystem>();
        sessionContext = GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>();
        
        netSystem.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        netSystem.AddMsgListener("MsgShowRoomList", OnMsgShowRoomList);
        netSystem.AddMsgListener("MsgJoinRoom", OnMsgJoinRoom);
        netSystem.AddMsgListener("MsgRoomInfo", OnMsgRoomInfo);
        netSystem.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        netSystem.AddMsgListener("MsgStartGame", OnMsgStartGame);
    }
    void OnDestroy()
    {
        
        netSystem.RemoveListener("MsgCreateRoom", OnMsgCreateRoom);
        netSystem.RemoveListener("MsgShowRoomList", OnMsgShowRoomList);
        netSystem.RemoveListener("MsgJoinRoom", OnMsgJoinRoom);
        netSystem.RemoveListener("MsgRoomInfo", OnMsgRoomInfo);
        netSystem.RemoveListener("MsgLeaveRoom", OnMsgLeaveRoom);
        netSystem.RemoveListener("MsgStartGame", OnMsgStartGame);
    }
    private void OnMsgStartGame(MsgBase msgBase)
    {
        MsgStartGame msgStartGame = (MsgStartGame)msgBase;
        if (msgStartGame.result == 1)
        {
            SceneManager.LoadScene("MainGameScene");
        }
        else
        {
            GameEntry.Instance.GetSystem<GlobalUiSystem>().ShowNotification("Start Game Failed", "Unable to start game. Please try again.");

        }
    }

    private void OnMsgLeaveRoom(MsgBase msgBase)
    {
        MsgLeaveRoom msgLeaveRoom = (MsgLeaveRoom)msgBase;
        if (msgLeaveRoom.result == 1)
        {
            sessionContext.RoomId = string.Empty;
            roomPanel.LeaveRoomSucc();
        }
        else
        {
            GameEntry.Instance.GetSystem<GlobalUiSystem>().ShowNotification("Leave Room Failed", "Unable to leave room. Please try again.");
        }
    }

    private void OnMsgRoomInfo(MsgBase msgBase)
    {
        MsgRoomInfo msgRoomInfo = (MsgRoomInfo)msgBase;
        roomPanel.UpdateRoomInfo(msgRoomInfo.roomInfo);
        sessionContext.RoomId = msgRoomInfo.roomInfo.roomId;
        sessionContext.opponentId = msgRoomInfo.roomInfo.playerIdList.Find(id => id != sessionContext.PlayeId);
    }

    private void OnMsgJoinRoom(MsgBase msgBase)
    {
        MsgJoinRoom msgJoinRoom = (MsgJoinRoom)msgBase;
        if (msgJoinRoom.result == 1)
        {
            sessionContext.RoomId = msgJoinRoom.roomId;
            roomPanel.JoinRoomSucc();
        }
        else
        {
            GameEntry.Instance.GetSystem<GlobalUiSystem>().ShowNotification("Join Room Failed", "Unable to join room. Please try again.");
        }
    }

    private void OnMsgShowRoomList(MsgBase msgBase)
    {
        roomListView.ShowRoomList(((MsgShowRoomList)msgBase).RoomList);
    }


    private void OnMsgCreateRoom(MsgBase msgBase)
    {
        MsgCreateRoom msgCreateRoom = (MsgCreateRoom)msgBase;
        if (msgCreateRoom.result == 1)
        {
            sessionContext.RoomId = msgCreateRoom.roomId;
            roomPanel.CreateRoomSucc();
        }
        else
        {
            GameEntry.Instance.GetSystem<GlobalUiSystem>().ShowNotification("Create Room Failed", "Unable to create room. Please try again.");
        }

    }

    public void ShowSettingPanel()
    {
        GameEntry.Instance.GetSystem<GlobalUiSystem>().ToggleGlobalSettingPanel();
    }

    public void StartGame()
    {
        Debug.Log("Start Game");
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
