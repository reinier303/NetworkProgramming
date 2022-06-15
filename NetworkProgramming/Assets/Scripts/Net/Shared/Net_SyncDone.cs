using Unity.Networking.Transport;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace NetworkProgramming
{
    public class Net_SyncDone : NetMessage
    {
        // 0 - 8 OP Code
        public int SyncDone;
        public int InitialSync;


        public Net_SyncDone()
        {
            Code = OpCode.SYNC_DONE;
        }
        public Net_SyncDone(DataStreamReader reader)
        {
            Code = OpCode.SYNC_DONE;
            Deserialize(reader);
        }
        public Net_SyncDone(int syncDone, int initialSync = 0)
        {
            Code = OpCode.SYNC_DONE;
            SyncDone = syncDone;
            InitialSync = initialSync;
        }

        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(SyncDone);
            writer.WriteInt(InitialSync);
        }

        public override void Deserialize(DataStreamReader reader)
        {
            //The first byte is handled already
            SyncDone = reader.ReadInt();
            InitialSync = reader.ReadInt();
        }

        public override void ReceivedOnServer(BaseServer server)
        {
            Debug.Log("SERVER::" + "SyncDone = " + SyncDone);
            server.Broadcast(this);
        }

        public virtual void ReceivedOnServer(AnotherSpaceGameServer server)
        {
            Debug.Log("SERVER::" + "SyncDone = " + SyncDone);

            server.Broadcast(this);
        }

        public override void ReceivedOnClient()
        {
            Debug.Log("CLIENT::" + "SyncDone = " + SyncDone);
        }
    }
}

