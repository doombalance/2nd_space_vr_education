//using System;
//using System.Collections;
//using UnityEngine;
//using Assets.Scripts.Singleton;
//using System.Collections.Generic;
//using Pr.ClientLib.Tcp;

//public class YGNetworkEngine : Singleton<YGNetworkEngine>
//{
//    private static ICommunication m_client = null;
//    private static Action<bool> OnConnectResultCallback = null;
//    private static Action<IClientNetReader> OnReceiveMsg_Callback = null;
//    private static Queue<IClientNetReader> msgQueue = new Queue<IClientNetReader>();

//    public static NetWorkEvent curNetWorkEvent = NetWorkEvent.None;
//    private readonly static object curNetWorkEventLock = new object();

//    /// <summary>
//    /// 链接
//    /// </summary>
//    /// <param name="host"></param>
//    /// <param name="port"></param>
//    /// <param name="Callback"></param>
//    public void Connect(string host, int port, Action<IClientNetReader> Callback = null)
//    {
//        OnReceiveMsg_Callback = Callback;
//        bool bflag = false;
//        if (null != m_client)
//        {
//            if (Connected)
//            {
//                Close();
//                bflag = true;
//            }
//        }
//        else
//        {
//            bflag = true;
//        }

//        if (bflag)
//        {
//            m_client = new Communicator();
//            m_client.OnServerInitiateClosing += OnClientDisConnected;
//            m_client.OnConnectSuccess += MClientOnOnConnectSuccess;
//        }
//        m_client.Connect(host, port);
//    }

//    private void MClientOnOnConnectSuccess(ICommunication communication)
//    {
//        m_client.OnMessageArrived -= OnMsgRecived;
//        m_client.OnMessageArrived += OnMsgRecived;
//        lock (curNetWorkEventLock)
//        {
//            curNetWorkEvent = NetWorkEvent.ConnectSuccess;
//        }
//    }

//    /// <summary>
//    /// 注册链接成功回调
//    /// </summary>
//    /// <param name="OnConnectResult"></param>
//    public void RegisterConnectResult(Action<bool> OnConnectResult)
//    {
//        OnConnectResultCallback = OnConnectResult;
//    }

//    public void UnRegisterConnectResult()
//    {
//        OnConnectResultCallback = null;
//    }

//    private void OnMsgRecived(IClientNetReader msg)
//    {
//        lock(msgQueue)
//            msgQueue.Enqueue(msg);
//    }

//    #region CallBack
//    private void OnClientConnected()
//    {
//        if (null != OnConnectResultCallback)
//        {
//            OnConnectResultCallback(true);
//        }
//    }

//    private void OnClientDisConnected(ICommunication netData)
//    {
//        lock (curNetWorkEventLock)
//        {
//            curNetWorkEvent = NetWorkEvent.DisConnect;
//        }
//    }

//    private void DoOnClientDisConnected()
//    {
//        if (null != OnConnectResultCallback)
//        {
//            OnConnectResultCallback(false);
//        }
//        else
//        {
//            ProcessUnConnected();
//        }
//    }
//    #endregion

//    private void Update()
//    {
//        lock (msgQueue)
//        {
//            while (msgQueue.Count > 0)
//            {
//                IClientNetReader msg = msgQueue.Dequeue();
//                if (OnReceiveMsg_Callback != null)
//                {
//                    OnReceiveMsg_Callback(msg);
//                }
//            }
//        }
//        lock (curNetWorkEventLock)
//        {
//            if (curNetWorkEvent == NetWorkEvent.ConnectSuccess)
//            {
//                OnClientConnected();
//                curNetWorkEvent = NetWorkEvent.None;
//            }
//            else if (curNetWorkEvent == NetWorkEvent.DisConnect)
//            {
//                DoOnClientDisConnected();
//                curNetWorkEvent = NetWorkEvent.None;
//            }
//        }
//    }

//    public void ProcessUnConnected()
//    {
//        if (StageManager.Instance.IsChanging)
//        {
//            return;
//        }

//        if (StageManager.Instance.CurStage != null && StageManager.Instance.CurStage.SceneType == EnumStageType.Battle)
//        {
//            return;
//        }

//        if (Connected)
//        {
//            return;
//        }

//        //处理断线重连
//        //MsgDlg.Show("提示", "网咯连接断开，是否继续连接", () =>
//        //{
//        //    OffLineReConnect();
//        //}, () =>
//        //{
//        //    //点取消，退出到登陆界面。
//        //    GoLoginStage();

//        //    //主动断开与gameSever的连接
//        //    Close();
//        //    UserInfo.Instance.Clear();
//        //});
//        //}
//        //else
//        //{
//        //    //MsgDlg.Show("提示", "当前网络不可用");
//        //}
//    }

//    public void OffLineReConnect()
//    {
//        YGUITools.Instance.StartYGCoroutine(OffLineWaitReConnect());
//    }

//    private IEnumerator OffLineWaitReConnect()
//    {
//        //YGUITools.Instance.ReLinkDialog.SetVisable(true);           //显示断线重连
//        UserInfo.Instance.m_bReContentedGameSetrver = false;
//        Close();

//        UserInfo.Instance.m_bReConLoginSetrver = true;

//        if (StageManager.Instance.CurStage != null && StageManager.Instance.CurStage.SceneType != EnumStageType.Battle)
//        {
//            UserInfo.Instance.m_bReConGameSetrver = false;
//        }
//        else
//        {
//            UserInfo.Instance.m_bReConGameSetrver = true;
//        }

//        yield return new WaitForSeconds(2f);

//        string strAccount = PlayerPreference.Instance.theAccountName;
//        string strPassword = PlayerPreference.Instance.thePassword;
//        string strQID = PlayerPreference.Instance.theQID;
//        string strTID = PlayerPreference.Instance.theTID;
//        string strPF = PlayerPreference.Instance.thePF;
//        string strIP = PlayerPreference.Instance.theIP;
//        string strPort = PlayerPreference.Instance.thePort;
//        string strPFKey = PlayerPreference.Instance.thePFKey;
//        UserInfo.Instance.m_strOpenId = strAccount + "@" + strQID;
//        UserInfo.Instance.m_strPF = strPF;
//        UserInfo.Instance.m_strServerIp = strIP;
//        UserInfo.Instance.m_nPort = int.Parse(strPort);
//        UserInfo.Instance.m_nServerId = int.Parse(strQID);
//        UserInfo.Instance.m_nTrueServerId = int.Parse(strTID);
//        UserInfo.Instance.m_strOpenKey = strPassword;

//        PlayerPreference.Instance.theAccountName = strAccount;
//        PlayerPreference.Instance.thePassword = strPassword;
//        PlayerPreference.Instance.theQID = strQID;
//        PlayerPreference.Instance.theTID = strTID;
//        PlayerPreference.Instance.thePF = strPF;
//        PlayerPreference.Instance.theIP = strIP;
//        PlayerPreference.Instance.thePort = strPort;
//        PlayerPreference.Instance.thePFKey = strPFKey;
//        RegisterConnectResult(OffLineReConnectResult);
//        Connect(UserInfo.Instance.m_strServerIp, UserInfo.Instance.m_nPort, MsgReciveer.Instance.OnReceiveMsg);
//    }

//    private void OffLineReConnectResult(bool bConnectSuccess)
//    {
//        UnRegisterConnectResult();
//        if (bConnectSuccess)
//        {
//            //stUserPreLogin prelogin = new stUserPreLogin();
//            //prelogin.SetVariable<uint>("dwCheckCode", 0x55884433);
//            //prelogin.SetVariable<float>("fclientver", 0);
//            //Send(prelogin);
//        }
//        else
//        {
//            OffLineReConnect();
//        }
//    }

//    /// <summary>
//    /// 在游戏中退出到登录界面 退出游戏,回到登录界面
//    /// </summary>
//    public void GoLoginStage()
//    {
//        UserInfo.Instance.Clear();
//        StageManager.Instance.ChangeStage<LoginStage>();
//    }

//    //public void Send()
//    //{
//    //    MyClient.Send();
//    //}

//    public void Close()
//    {
//        if (m_client != null && Connected)
//        {
//            m_client.Close();
//            m_client.OnMessageArrived -= OnMsgRecived;
//            m_client.OnServerInitiateClosing -= OnClientDisConnected;
//            m_client = null;
//        }
//    }

//    public ICommunication MyClient
//    {
//        get
//        {
//            return m_client;
//        }
//    }

//    public bool Connected
//    {
//        get
//        {
//            if (null != m_client)
//            {
//                return m_client.Connected;
//            }
//            return false;
//        }
//    }

//    /// <summary>
//    /// 当程序退出时，断开与服务器的连接
//    /// </summary>
//    private new void OnApplicationQuit()
//    {
//        Close();
//    }
//}