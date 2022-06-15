using Unity.Networking.Transport;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace NetworkProgramming
{
    public class Net_PowerupType : NetMessage
    {
        // 0 - 8 OP Code
        public int Amount;
        public List<int> Powerups;

        public Net_PowerupType()
        {
            Code = OpCode.POWERUP_TYPE;
        }
        public Net_PowerupType(DataStreamReader reader)
        {
            Code = OpCode.POWERUP_TYPE;
            Deserialize(reader);
        }
        public Net_PowerupType(int amount, List<int> powerups)
        {
            Code = OpCode.POWERUP_TYPE;
            Amount = amount;
            Powerups = powerups;
        }

        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(Amount);
            for (int i = 0; i < Amount; i++)
            {
                writer.WriteInt(Powerups[i]);
            }
        }

        public override void Deserialize(DataStreamReader reader)
        {
            //The first byte is handled already
            Amount = reader.ReadInt();
            Powerups = new List<int>();
            for (int i = 0; i < Amount; i++)
            {
                Powerups.Add(reader.ReadInt());
            }
        }

        public override void ReceivedOnServer(BaseServer server)
        {
            Debug.Log("SERVER::" + "powerupSelected");
            server.Broadcast(this);
        }

        public virtual void ReceivedOnServer(AnotherSpaceGameServer server)
        {
            Debug.Log("SERVER::" + "powerupSelected");

            server.Broadcast(this);
        }

        public override void ReceivedOnClient()
        {
            Debug.Log("CLIENT::" + "powerupSelected");
        }
    }
}