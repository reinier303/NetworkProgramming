using Unity.Networking.Transport;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace NetworkProgramming
{
    public class Net_BulletData : NetMessage
    {
        // 0 - 8 OP Code
        public int PlayerId { set; get; }
        public float PositionX, PositionY;
        public float RotationZ;
        public float Damage, Speed;


        public Net_BulletData()
        {
            Code = OpCode.BULLET_DATA;
        }
        public Net_BulletData(DataStreamReader reader)
        {
            Code = OpCode.BULLET_DATA;
            Deserialize(reader);
        }
        public Net_BulletData(int playerId, float x, float y, float rotationZ, float damage, float speed)
        {
            Code = OpCode.BULLET_DATA;
            PlayerId = playerId;
            PositionX = x;
            PositionY = y;
            RotationZ = rotationZ;
            Damage = damage;
            Speed = speed;
        }

        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(PlayerId);
            writer.WriteFloat(PositionX);
            writer.WriteFloat(PositionY);
            writer.WriteFloat(RotationZ);
            writer.WriteFloat(Damage);
            writer.WriteFloat(Speed);
        }

        public override void Deserialize(DataStreamReader reader)
        {
            //The first byte is handled already
            PlayerId = reader.ReadInt();
            PositionX = reader.ReadFloat();
            PositionY = reader.ReadFloat();
            RotationZ = reader.ReadFloat();
            Damage = reader.ReadFloat();
            Speed = reader.ReadFloat();
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