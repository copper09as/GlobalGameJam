using System;
using GameFramework;
using Michsky.MUIP;
using TMPro;
using UnityEngine;

public class RoomPanel : MonoBehaviour
{
    public TMP_Text RoomId;
    public TMP_Text Player1;
    public TMP_Text Player2;
    [SerializeField]private ButtonManager CreateRoomButton;
    [SerializeField]private ButtonManager LeaveRoomButton;
    
    [SerializeField]private ButtonManager StartGameButton;
    void Start()
    {
        CreateRoomButton.onClick.AddListener(CreateRoomBtnClicked);
        LeaveRoomButton.onClick.AddListener(LeaveRoomBtnClicked);
        StartGameButton.onClick.AddListener(StartGameBtnClicked);
    }

    private void StartGameBtnClicked()
    {
        MsgStartGame msg = new MsgStartGame();
        GameEntry.Instance.GetSystem<NetSystem>().Send(msg);
    }

    public void CreateRoomSucc()
    {
    }
    public void LeaveRoomSucc()
    {
        RoomId.text = "";
        Player1.text = "";
        Player2.text = "";
    }
    public void CreateRoomBtnClicked()
    {
        MsgCreateRoom msg = new MsgCreateRoom();
        GameEntry.Instance.GetSystem<NetSystem>().Send(msg);
    }
    public void UpdateRoomInfo(RoomInfo roomInfo)
    {
        RoomId.text = $"Room ID: {roomInfo.roomId}";
        Player1.text = roomInfo.playerIdList.Count > 0 ? roomInfo.playerIdList[0] : "Waiting...";
        Player2.text = roomInfo.playerIdList.Count > 1 ? roomInfo.playerIdList[1] : "Waiting...";
    }
    public void LeaveRoomBtnClicked()
    {
        MsgLeaveRoom msg = new MsgLeaveRoom();
        GameEntry.Instance.GetSystem<NetSystem>().Send(msg);
    }

    public void JoinRoomSucc()
    {
        
    }
}