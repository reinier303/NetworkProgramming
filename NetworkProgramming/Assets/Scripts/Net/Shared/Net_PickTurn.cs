using Unity.Networking.Transport;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace NetworkProgramming
{
    public class Net_PickTurn : NetMessage
    {
        // 0 - 8 OP Code
        public int Turn { set; get; }

        public Net_PickTurn()
        {
            Code = OpCode.PICK_TURN;
        }
        public Net_PickTurn(DataStreamReader reader)
        {
            Code = OpCode.PICK_TURN;
            Deserialize(reader);
        }
        public Net_PickTurn(int turn)
        {
            Code = OpCode.PICK_TURN;
            Turn = turn;
        }

        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(Turn);
        }

        public override void Deserialize(DataStreamReader reader)
        {
            //The first byte is handled already
            Turn = reader.ReadInt();
        }

        public override void ReceivedOnServer(BaseServer server)
        {
            Debug.Log("SERVER::" + Turn + "Turn");
            server.Broadcast(this);
        }

        public virtual void ReceivedOnServer(AnotherSpaceGameServer server)
        {
            Debug.Log("SERVER::" + Turn + "Turn");
            server.Broadcast(this);
        }

        public override void ReceivedOnClient()
        {
            Debug.Log("CLIENT::" + Turn + "Turn");
        }
    }
}