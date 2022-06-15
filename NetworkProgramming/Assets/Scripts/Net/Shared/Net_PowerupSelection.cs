using Unity.Networking.Transport;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace NetworkProgramming
{
    public class Net_PowerupSelection : NetMessage
    {
        // 0 - 8 OP Code
        public int PlayerId { set; get; }
        public int Selection;

        public Net_PowerupSelection()
        {
            Code = OpCode.POWERUP_SELECTION;
        }
        public Net_PowerupSelection(DataStreamReader reader)
        {
            Code = OpCode.POWERUP_SELECTION;
            Deserialize(reader);
        }
        public Net_PowerupSelection(int playerId, int selection)
        {
            Code = OpCode.POWERUP_SELECTION;
            PlayerId = playerId;
            Selection = selection;
        }

        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(PlayerId);
            writer.WriteInt(Selection);
        }

        public override void Deserialize(DataStreamReader reader)
        {
            //The first byte is handled already
            PlayerId = reader.ReadInt();
            Selection = reader.ReadInt();
        }

        public override void ReceivedOnServer(BaseServer server)
        {
            Debug.Log("SERVER::" + PlayerId + ", " + Selection);
            server.Broadcast(this);
        }

        public virtual void ReceivedOnServer(AnotherSpaceGameServer server)
        {
            Debug.Log("SERVER::" + PlayerId + ", " + Selection);
            server.Broadcast(this);
        }

        public override void ReceivedOnClient()
        {
            Debug.Log("CLIENT::" + PlayerId + ", " + Selection);
        }
    }
}