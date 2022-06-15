using Unity.Networking.Transport;
using UnityEngine;

namespace NetworkProgramming
{
    public class NetMessage
    {
        public OpCode Code { set; get; }

        public virtual void Serialize(ref DataStreamWriter writer)
        {
            //This method is meant to be overidden.
        }

        public virtual void Deserialize(DataStreamReader reader)
        {
            //This method is meant to be overidden.
        }

        public virtual void ReceivedOnClient()
        {

        }

        public virtual void ReceivedOnServer(BaseServer server)
        {

        }
    }
}