//using Assets.Scripts.Singleton;

//public class YGConnecter : SingletonTemplate<YGConnecter>
//{
//    /// <summary>
//    /// 连接登陆服务器
//    /// </summary>
//    public void ConnectLoginServer()
//    {
//        YGNetworkEngine.Instance.RegisterConnectResult(OnConnectLoginServerResult);
//        YGNetworkEngine.Instance.Connect(UserInfo.Instance.m_strServerIp, UserInfo.Instance.m_nPort, MsgReciveer.Instance.OnReceiveMsg);
//    }

//    /// <summary>
//    /// 连接Game服务器
//    /// </summary>
//    public void ConnectGameServer()
//    {
//        YGNetworkEngine.Instance.Close();
//        YGNetworkEngine.Instance.RegisterConnectResult(OnConnectGameServerResult);
//        YGNetworkEngine.Instance.Connect(UserInfo.Instance.m_strServerIp, UserInfo.Instance.m_nPort, MsgReciveer.Instance.OnReceiveMsg);
//    }

//    private void OnConnectLoginServerResult(bool bConnectSuccess)
//    {
//        if (bConnectSuccess)
//            OnConnectGameServerResult(bConnectSuccess);
//        else
//            GameDebugLog.Log("Socket已经断开连接");
//    }

//    public void OnConnectGameServerResult(bool bConnectSuccess)
//    {
//        doClientLoginCmd client = ClientCmdManager.Instance.GetClientCmd(ECmdType.CMD_LOGIN) as doClientLoginCmd;
//        if (null != client)
//        {
//            client.ClientLoginGame(UserInfo.Instance.m_strOpenId, UserInfo.Instance.m_strOpenKey);
//        }
//    }

//}