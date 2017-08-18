using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using Pr.ClientLib.Tcp;
using System;

public class NetworkEngine
{
    private static ICommunication m_client = null;
    private static Action<bool> OnConnectResultCallback = null;
    private static Action<IClientNetReader> OnReceiveMsgCallback = null;
    private static Queue<IClientNetReader> msgQueue = new Queue<IClientNetReader>();

    public static NetWorkEvent curNetWorkEvent = NetWorkEvent.None;
    private readonly static object curNetWorkEventLock = new object();

    private static NetworkEngine singleton = null;


    #region Singleton
    private NetworkEngine()
    {

    }

    public static NetworkEngine getInstance()
    {
        if(singleton == null)
        {
            singleton = new NetworkEngine();
        }

        return singleton;
    }
    #endregion

    //获取网络收发对象
    public ICommunication MyClient
    {
        get
        {
            return m_client;
        }
    }

    //是否已连接
    public bool Connected
    {
        get
        {
            if (null != m_client)
            {
                return m_client.Connected;
            }
            return false;
        }
    }

    //连接服务器
    public void Connect(string host, int port, Action<IClientNetReader> Callback = null)
    {
        OnReceiveMsgCallback = Callback;
        bool bflag = false;
        if (null != m_client)
        {
            if (Connected)
            {
                Close();
                bflag = true;
            }
        }
        else
        {
            bflag = true;
        }

        if (bflag)
        {
            m_client = new Communicator();
            m_client.OnServerInitiateClosing += OnClientDisConnected;
            m_client.OnConnectSuccess += MClientOnOnConnectSuccess;
        }
        m_client.Connect(host, port);
    }

    //断开连接
    public void Close()
    {
        if (m_client != null && Connected)
        {
            m_client.Close();
            m_client.OnMessageArrived -= OnMsgRecieved;
            m_client.OnServerInitiateClosing -= OnClientDisConnected;
            m_client = null;
        }
    }

    //注册链接成功回调
    public void RegisterConnectResult(Action<bool> OnConnectResult)
    {
        OnConnectResultCallback = OnConnectResult;
    }

    public void UnRegisterConnectResult()
    {
        OnConnectResultCallback = null;
    }



    private void MClientOnOnConnectSuccess(ICommunication communication)
    {
        m_client.OnMessageArrived -= OnMsgRecieved;
        m_client.OnMessageArrived += OnMsgRecieved;
        lock (curNetWorkEventLock)
        {
            curNetWorkEvent = NetWorkEvent.ConnectSuccess;
        }
    }

    private void OnMsgRecieved(IClientNetReader msg)
    {
        lock (msgQueue)
            msgQueue.Enqueue(msg);
    }

    #region CallBack
    private void OnClientConnected()
    {
        if (null != OnConnectResultCallback)
        {
            OnConnectResultCallback(true);
        }
    }

    private void OnClientDisConnected(ICommunication netData)
    {
        lock (curNetWorkEventLock)
        {
            curNetWorkEvent = NetWorkEvent.DisConnect;
        }
    }

    private void DoOnClientDisConnected()
    {
        if (null != OnConnectResultCallback)
        {
            OnConnectResultCallback(false);
        }
        else
        {
            //ProcessUnConnected();
        }
    }
    #endregion
}

public enum NetWorkEvent
{
    None,
    ConnectSuccess,
    DisConnect
}
