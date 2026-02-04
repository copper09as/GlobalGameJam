using System.Collections.Generic;
using GameFramework;
using UnityEngine;

public class RoomListView : MonoBehaviour
{
    [SerializeField] private GameObject roomInfoPrefab;
    [SerializeField] List<RoomInfoUi> roomInfoUiList = new();
    [SerializeField] private Transform roomListRoot;   // Content
    void OnEnable()
    {
        MsgShowRoomList msg = new MsgShowRoomList();
        GameEntry.Instance.GetSystem<NetSystem>().Send(msg);
    }
    public void ShowRoomList(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = roomInfoUiList.Count; i < roomCount; i++)
        {
            GameObject go = Instantiate(roomInfoPrefab, roomListRoot);
            RoomInfoUi ui = go.GetComponent<RoomInfoUi>();
            roomInfoUiList.Add(ui);
        }
        for (int i = roomInfoUiList.Count - 1; i >= roomCount; i--)
        {
            Destroy(roomInfoUiList[i].gameObject);
            roomInfoUiList.RemoveAt(i);
        }
        for (int i = 0; i < roomCount; i++)
        {
            roomInfoUiList[i].gameObject.SetActive(true);
            roomInfoUiList[i].SetData(roomList[i]);
        }
    }
}
