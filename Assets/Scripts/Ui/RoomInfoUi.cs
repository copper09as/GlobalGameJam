using System;
using GameFramework;
using Michsky.MUIP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfoUi : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text roomIdText;        // 房间ID
    public TMP_Text playerCountText;  // 玩家数量
    public ButtonManager joinButton;         // 加入按钮

    private string roomId;

    void Awake()
    {
        joinButton.onClick.AddListener(OnClickJoin);
    }


    /// <summary>
    /// 由房间列表控制器调用，刷新UI
    /// </summary>
    public void SetData(RoomInfo info)
    {
        roomId = info.roomId;

        roomIdText.text = $"RoomId: {info.roomId}";
        playerCountText.text = $"Player: {info.playerIdList.Count}";

        // 满员时不可加入（可选）
        bool isFull = info.playerIdList.Count >= 2;
        joinButton.isInteractable = !isFull;
    }

    /// <summary>
    /// 绑定到 Join Button 的 OnClick
    /// </summary>
    public void OnClickJoin()
    {
        if (string.IsNullOrEmpty(roomId))
        {
            Debug.LogWarning("RoomId is empty, cannot join room.");
            return;
        }
        MsgJoinRoom msg = new MsgJoinRoom();
        msg.roomId = roomId;
        GameEntry.Instance.GetSystem<NetSystem>().Send(msg);

        Debug.Log($"[RoomInfoUi] 请求加入房间: {roomId}");
    }
}
