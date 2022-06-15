using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

namespace NetworkProgramming
{
    public class BaseServer : MonoBehaviour
    {
        //Interface for anything network related
        public NetworkDriver driver;

        //List of connections
        protected NativeList<NetworkConnection> connections;

        private void Start() { Init(); }

        private void Update() { UpdateServer(); }

        private void OnDestroy() { ShutDown(); }

        public virtual void Init()
        {
            //INITIALISE THE DRIVER

            driver = NetworkDriver.Create();

            //WHO can connect to us?
            NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;

            endpoint.Port = 2711;
            if (driver.Bind(endpoint) != 0)
            {
                //Will return error if port is already used by another application
                Debug.Log("Error binding to port " + endpoint.Port);
            }
            else
            {
                //Open the server to communication listening
                driver.Listen();
            }

            //INITIALISE THE CONNECTION LIST

            //Setup max amount of concurrent players
            connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        }

        public virtual void ShutDown()
        {
            //Clear server and connections
            driver.Dispose();
            connections.Dispose();
        }

        public virtual void UpdateServer()
        {
            //Used for JOBS system to work
            driver.ScheduleUpdate().Complete();

            //Clean up connection when dropping out 
            CleanupConnection();

            //Check new players connections
            AcceptNewConnection();

            //Parse messages client is sending
            UpdateMessagePump();
        }

        private void CleanupConnection()
        {
            for (int i = 0; i < connections.Length; i++)
            {
                if (!connections[i].IsCreated)
                {
                    connections.RemoveAtSwapBack(i);
                    i--;
                }
            }
        }

        protected virtual void AcceptNewConnection()
        {
            NetworkConnection c;
            while ((c = driver.Accept()) != default(NetworkConnection))
            {
                connections.Add(c);
                Debug.Log("Accepted a connection");
            }
        }

        private void UpdateMessagePump()
        {
            DataStreamReader stream;
            for (int i = 0; i < connections.Length; i++)
            {
                //Creat event and check if its not empty. if not return data if type is data and disconnect if type disconnect
                NetworkEvent.Type cmd;
                while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
                {
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        OnData(stream);
                    }
                    else
                    {
                        Debug.Log("Client disconnected from the server");
                        connections[i] = default(NetworkConnection);
                    }
                }
            }
        }

        public virtual void OnData(DataStreamReader stream)
        {
            NetMessage msg = null;

            //Read first byte from data stream to uncover type
            var opCode = (OpCode)stream.ReadByte();

            switch (opCode)
            {
                case OpCode.CHAT_MESSAGE: msg = new Net_ChatMessage(stream); break;
                case OpCode.PLAYER_POSITION: msg = new Net_PlayerPosition(stream); break;
                //case OpCode.BULLET_POSITION: msg = new Net_BulletPosition(stream); break;
                //case OpCode.POWERUP_TYPE: msg = new Net_PowerupType(stream); break;
                default: Debug.Log("Message received without OpCode"); break;
            }

            msg.ReceivedOnServer(this);
        }

        /// <summary>
        /// Send data to all connected clients
        /// </summary>
        /// <param name="msg"></param>
        public virtual void Broadcast(NetMessage msg)
        {
            for (int i = 0; i < connections.Length; i++)
            {
                if (connections[i].IsCreated)
                {
                    SendToClient(connections[i], msg);
                }
            }
        }

        public virtual void SendToClient(NetworkConnection connection, NetMessage msg)
        {
            DataStreamWriter writer;
            //Begin sending and connect writer to connection
            driver.BeginSend(connection, out writer);
            //Fill with data
            msg.Serialize(ref writer);
            //Send the message
            driver.EndSend(writer);
        }
    }
}