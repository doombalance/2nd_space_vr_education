//using Assets.Scripts.Singleton;
//using System;
//using System.Collections.Generic;
//using Pr.ClientLib.Tcp;

//public class MsgReciveer : SingletonTemplate<MsgReciveer>
//{
//    public int cmdValue;
//    private ushort subFunctionId;

//    public void OnReceiveMsg(IClientNetReader netData)
//    {
//        if (netData == null)
//        {
//            GameDebugLog.LogError("netData == null", true);
//            return;
//        }

//        ReadMsgId(netData.ReadInt16());
//        if (cmdValue == 0xFFFF || subFunctionId == 255)  //心跳包
//        {
//            var myClient = YGNetworkEngine.Instance.MyClient.CreateNewWriter();
//            myClient.WriteInt16(short.MaxValue);
//            myClient.Send();
//        }
//        else
//        {
//            ClientCmdBase client = ClientCmdManager.Instance.GetClientCmd((ECmdType)subFunctionId);
//            if (null != client)
//            {
//                client.msgParse(netData);
//            }
//        }
//    }

//    private void ReadMsgId(int value)
//    {
//        cmdValue = value;
//        subFunctionId = (ushort)(value & 0x00FF);
//    }
//}

//internal class ClientCmdManager : SingletonTemplate<ClientCmdManager>
//{
//    private Dictionary<ECmdType, ClientCmdBase> m_ClientCmdDic = new Dictionary<ECmdType, ClientCmdBase>();

//    public ClientCmdBase GetClientCmd(ECmdType cmd)
//    {
//        if (!m_ClientCmdDic.ContainsKey(cmd))
//        {
//            try
//            {
//                ClientCmdBase client = Activator.CreateInstance(Type.GetType(cmd.GetStringValue())) as ClientCmdBase;
//                if (null != client)
//                {
//                    m_ClientCmdDic[cmd] = client;
//                }
//                else
//                {
//                    GameDebugLog.LogError("服务器下发的包类型没有对应的实例可以创建!" + cmd.ToString(), true);
//                    return null;
//                }
//            }
//            catch(Exception ex)
//            {
//                return null;
//            }
            
//        }
//        return m_ClientCmdDic[cmd];
//    }
//}