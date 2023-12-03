using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetSendName : NetMessage
{
    public string name { set; get; }

    public NetSendName(string msg)
    {
        Code = OpCode.SEND_NAME;
        name = msg;
    }

    public NetSendName(DataStreamReader reader)
    {
        Code = OpCode.SEND_NAME;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteFixedString128(name);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        int length = reader.Length;
        byte[] data = new byte[length];
        var nativeArray = new NativeArray<byte>(data, Allocator.Temp);
        reader.ReadBytes(nativeArray);
        name = System.Text.Encoding.UTF8.GetString(nativeArray);
        Debug.Log("Deserialized name " + name);
    }
    public override void ReceivedOnClient()
    {
        NetUtility.C_SEND_NAME?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_SEND_NAME?.Invoke(this, cnn);
    }
}
