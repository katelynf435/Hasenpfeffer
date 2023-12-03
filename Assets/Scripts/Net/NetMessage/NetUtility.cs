using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public enum OpCode
{
    KEEP_ALIVE = 1,
    SEND_NAME = 2,
    WELCOME = 3,
    SEND_TEAM = 4,
    START_GAME = 5,
    UPDATE_DATA = 6
}

public static class NetUtility
{
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_SEND_NAME;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_UPDATE_DATA;
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_SEND_NAME;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_UPDATE_DATA;

    public static void OnData(DataStreamReader stream, NetworkConnection cnn, Server server = null)
    {
        NetMessage msg = null;
        var opCode = (OpCode)stream.ReadByte();
        switch (opCode)
        {
            case OpCode.KEEP_ALIVE: msg = new NetKeepAlive(stream); break;
            case OpCode.SEND_NAME: msg = new NetSendName(stream); break;
            case OpCode.WELCOME: msg = new NetWelcome(stream); break;
        //    case OpCode.START_GAME: msg = new NetStartGame(stream); break;
        //    case OpCode.UPDATE_DATA: msg = new NetUpdateData(stream); break;
            default:
                Debug.LogError("Message received had no OpCpde");
                break;
        }

        if (server != null)
            msg.ReceivedOnServer(cnn);
        else
            msg.ReceivedOnClient();
    }
}
