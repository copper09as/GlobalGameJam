using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using GameFramework;
using UnityEngine;
public class NetSystem:IGameSystem
{
	private Socket socket;
	private ByteArray readBuff;
	private Queue<ByteArray> writeQueue;
    private bool isClosing = false;
	private List<MsgBase> msgList = new List<MsgBase>();
	private int msgCount = 0;
	readonly private int MAX_MESSAGE_FIRE = 100;
    public delegate void EventListener(string err);
    private  Dictionary<NetEvent, EventListener> eventListeners =
		new Dictionary<NetEvent, EventListener>();
    public delegate void MsgListener(MsgBase msgBase);
	private Dictionary<string, MsgListener> msgListener =
		new Dictionary<string, MsgListener>();
	public bool isUsePing = true;
	public int pingInterval = 10;
	public float lastPongTime = 0;
    public float lastPingTime = 0;


    public void AddMsgListener(string msgName,MsgListener listener)
	{
		if(msgListener.ContainsKey(msgName))
		{
			msgListener[msgName] += listener;
		}
		else
		{
			msgListener[msgName] = listener;
		}
	}
	public void RemoveListener(string msgName,MsgListener listener)
	{
        if (msgListener.ContainsKey(msgName))
        {
            msgListener[msgName] -= listener;
			if (msgListener[msgName]==null)
			{
				msgListener.Remove(msgName);
			}
        }
    }
	private void FireMsg(string msgName, MsgBase msgBase)
	{
		RunOnMainThread(() =>
		{
			if (msgListener.ContainsKey(msgName))
			{
				msgListener[msgName]?.Invoke(msgBase);
			}
		});
	}
	public void AddEventListener(NetEvent netEvent,EventListener Listener)
	{
		if(eventListeners.ContainsKey(netEvent))
		{
			eventListeners[netEvent] += Listener;
		}
		else
		{
            eventListeners[netEvent] = Listener;
        }
	}
	public void RemoveEventListener(NetEvent netEvent, EventListener Listener)
	{
		if(eventListeners.ContainsKey(netEvent))
		{
            eventListeners[netEvent] -= Listener;
            if (eventListeners[netEvent] == null)
            {
                eventListeners.Remove(netEvent);
            }
        }

    }
	private readonly Queue<Action> mainThreadActions = new Queue<Action>();

	private void RunOnMainThread(Action action)
	{
		lock (mainThreadActions)
		{
			mainThreadActions.Enqueue(action);
		}
	}
	private void FireEvent(NetEvent netEvent, string err)
	{
		RunOnMainThread(() =>
		{
			UnityEngine.Debug.Log($"[MainThread] FireEvent: {netEvent}");
			if (eventListeners.ContainsKey(netEvent))
			{
				eventListeners[netEvent]?.Invoke(err);
			}
		});
	}

	public bool isConnecting = false;

    public int Priority => 0;

    public void Connect(string ip,int port)
	{
		if(socket!=null&&socket.Connected)
		{
			return;
		}
		if (isConnecting)
		{
            return;
        }
		InitState();
		socket.NoDelay = true;
		isConnecting = true;
		socket.BeginConnect(ip,port,ConnectCallback,socket);
		UnityEngine.Debug.Log("Begin Connect");
	}

    private void ConnectCallback(IAsyncResult ar)
    {
        try
		{
			Socket socket = (Socket)ar.AsyncState;
			socket.EndConnect(ar);
            FireEvent(NetEvent.ConnectSucc,"");
			isConnecting = false;
			socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
		}
		catch(SocketException ex)
		{
			FireEvent(NetEvent.ConnectFail, ex.ToString());
			isConnecting = false;
		}
    }
    private void ReceiveCallback(IAsyncResult ar)
    {
		try
		{
            Socket socket = (Socket)ar.AsyncState;
			int count = socket.EndReceive(ar);
			if(count==0)
			{
				Close();
				return;
			}
			readBuff.writeIdx += count;
			OnReceiveDate();
			if(readBuff.remain<8)
			{
				readBuff.MoveBytes();
				readBuff.ReSize(readBuff.length * 2);
			}
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
		catch(SocketException ex)
		{
			UnityEngine.Debug.Log("ReceiveCallback caught SocketException: " + ex.ToString());
			FireEvent(NetEvent.Close, ex.ToString());
			UnityEngine.Debug.Log("Socket Receive Fail" + ex.ToString());
		}

    }

    private void OnReceiveDate()
    {
		
        if(readBuff.length<=2)
		{
            return;
		}
		int readIdx = readBuff.readIdx;
		byte[] bytes = readBuff.bytes;
		Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);

        if (readBuff.length<bodyLength+2)
		{
			return;
		}
		readBuff.readIdx += 2;
		int nameCount = 0;
		string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        if (protoName == "")
		{
			UnityEngine.Debug.Log("OnReceiveData Msg.DecodeName Fail");
			return;
		}
		readBuff.readIdx += nameCount;
		int bodyCount = bodyLength - nameCount;
		MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
		readBuff.readIdx += bodyCount;
		readBuff.CheckAndMoveBytes();
		lock(msgList)
		{
			msgList.Add(msgBase);
		}
		msgCount++;
		if(readBuff.length>2)
		{
			OnReceiveDate();
		}
    }
	private void MsgUpdate()
	{
		if (msgCount == 0)
		{
            
            return;
		}
        for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
		{
            MsgBase msgBase = null;
			lock(msgList)
			{
				if(msgList.Count>0)
				{
					msgBase = msgList[0];
					msgList.RemoveAt(0);
					msgCount--;
				}
			}
			if(msgBase!=null)
			{
				FireMsg(msgBase.protoName, msgBase);
			}
			else
			{
				break;
			}
		}
	}
    public void Close()
	{
        if (socket==null || !socket.Connected)
        {
			return;
        }
		if(isConnecting)
		{
			return;
		}
		if(writeQueue.Count>0)
		{
			isClosing = true;
		}
		else
		{
			socket.Close();
			FireEvent(NetEvent.Close, "");
		}
    }
	public void Send(MsgBase msg)
	{
        if (socket == null || !socket.Connected)
        {
            return;
        }
        if (isConnecting)
        {
            return;
        }
		if (isClosing)
		{
			return;
		}
		byte[] nameBytes = MsgBase.EncodeName(msg);
		byte[] bodyBytes = MsgBase.Encode(msg);
		int len = nameBytes.Length + bodyBytes.Length;
		byte[] sendBytes = new byte[2 + len];
		sendBytes[0] = (byte)(len % 256);
		sendBytes[1] = (byte)(len / 256);
		Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
		Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
		ByteArray ba = new ByteArray(sendBytes);
		int count = 0;
		lock(writeQueue)
		{
			writeQueue.Enqueue(ba);
			count = writeQueue.Count;
		}
		if(count == 1)
		{
			socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallBack, socket);
		}
    }

    private void SendCallBack(IAsyncResult ar)
    {
		Socket socket = (Socket)ar.AsyncState;
        if (socket == null || !socket.Connected)
        {
            return;
        }

		int count = socket.EndSend(ar);
		ByteArray ba;
		lock(writeQueue)
		{
			if (writeQueue.Count == 0)
			{
				return;
			}
			ba = writeQueue.Peek();
			ba.readIdx += count;
			if (ba.length == 0)
			{
				writeQueue.Dequeue();
				if (writeQueue.Count > 0)
				{
					ba = writeQueue.Peek();
				}
				else
				{
					ba = null;
				}
			}
		}
		if(ba!=null)
		{
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallBack, socket);
        }
		else if(isClosing)
		{
			socket.Close();
		}
    }
    private void PingUpdate()
    {
        if (!isUsePing)
        {
            return;
        }

        // 先更新时间戳
        float timeSinceLastPing = Time.time - lastPingTime;

        // 判断是否超过间隔
        if (timeSinceLastPing >= pingInterval)
        {
            MsgPing msgPing = new MsgPing();
            Send(msgPing);
            lastPingTime = Time.time; // 更新时间戳
            UnityEngine.Debug.Log("Ping");
        }

        // 保持Pong超时检测逻辑不变
        if (Time.time - lastPongTime > pingInterval * 4)
        {
            Close();
        }
    }
    private void OnMsgPong(MsgBase msgBase)
	{
		lastPongTime = Time.time;
	}
    private void InitState()
    {
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		readBuff = new ByteArray();
		writeQueue = new Queue<ByteArray>();
		isConnecting = false;
		isClosing = false;
		msgList = new List<MsgBase>();
		msgCount = 0;
		lastPongTime = Time.time;
		lastPingTime = Time.time;
		if(!msgListener.ContainsKey("MsgPong"))
		{
			AddMsgListener("MsgPong", OnMsgPong);
		}
    }

    public void OnInit()
	{
		
	}

	public void OnUpdate(float deltaTime)
	{
		while (mainThreadActions.Count > 0)
		{
			Action action;
			lock (mainThreadActions)
			{
				action = mainThreadActions.Dequeue();
			}
			action?.Invoke();
		}

		MsgUpdate();
		PingUpdate();
	}


    public void OnShutdown()
	{
		Close();
	}
}
	

public enum NetEvent
{
	ConnectSucc = 1,
	ConnectFail = 2,
	Close = 3
}