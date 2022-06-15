using Unity.Networking.Transport;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace NetworkProgramming
{
    public class Net_PlayerPosition : NetMessage
    {
        // 0 - 8 OP Code
        public int PlayerId { set; get; }
        public float PositionX, PositionY;

        public Net_PlayerPosition()
        {
            Code = OpCode.PLAYER_POSITION;
        }
        public Net_PlayerPosition(DataStreamReader reader)
        {
            Code = OpCode.PLAYER_POSITION;
            Deserialize(reader);
        }
        public Net_PlayerPosition(int playerId, float x, float y)
        {
            Code = OpCode.PLAYER_POSITION;
            PlayerId = playerId;
            PositionX = x;
            PositionY = y;
        }

        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(PlayerId);
            writer.WriteFloat(PositionX);
            writer.WriteFloat(PositionY);
        }

        public override void Deserialize(DataStreamReader reader)
        {
            //The first byte is handled already
            PlayerId = reader.ReadInt();
            PositionX = reader.ReadFloat();
            PositionY = reader.ReadFloat();
        }

        public override void ReceivedOnServer(BaseServer server)
        {
            //Debug.Log("SERVER::" + PlayerId + ", " + PositionX + ", " + PositionY);
            server.Broadcast(this);
        }

        public virtual void ReceivedOnServer(AnotherSpaceGameServer server)
        {
            Debug.Log("SERVER::" + PlayerId + ", " + PositionX + ", " + PositionY);
            server.Broadcast(this);
        }

        public override void ReceivedOnClient()
        {
            Debug.Log("CLIENT::" + PlayerId + ", " + PositionX + ", " + PositionY);
        }
    }
}