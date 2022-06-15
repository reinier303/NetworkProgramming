using Unity.Collections;
using UnityEngine;
using Unity.Networking.Transport;

namespace NetworkProgramming
{
    public class BaseClient : MonoBehaviour
    {
        public string ipAddress = "127.0.0.1";
        public ushort port = 2711;

        //Interface for anything network related
        public NetworkDriver driver;

        protected NetworkConnection connection;

        [Header("Testing")]
        public bool LoopBack = false;

        private void Start() { Init(); }

        private void Update() { UpdateClient(); }

        private void OnDestroy() { ShutDown(); }

        public virtual void Init()
        {
            //INITIALISE THE DRIVER

            driver = NetworkDriver.Create();
            connection = default(NetworkConnection);

            //WHERE to connect?
            NetworkEndPoint endpoint = NetworkEndPoint.Parse(ipAddress, port);

            if(LoopBack)
            {
                endpoint = NetworkEndPoint.LoopbackIpv4;
                endpoint.Port = port;
            }

            connection = driver.Connect(endpoint);
        }

        public virtual void ShutDown()
        {
            //Clear server and connections
            driver.Dispose();
        }

        public virtual void UpdateClient()
        {
            //Used for JOBS system to work
            driver.ScheduleUpdate().Complete();

            CheckAlive();

            //Parse messages client is sending
            UpdateMessagePump();
        }

        private void CheckAlive()
        {
            if (!connection.IsCreated)
            {
                Debug.Log("Something went wrong, lost connection to server");
            }
        }

        private void UpdateMessagePump()
        {
            DataStreamReader stream;
            //Create event and check if its not empty. if not return data if type is data and disconnect if type disconnect
            NetworkEvent.Type cmd;
            while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    Debug.Log("Connection to server success");
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    OnData(stream);
                }
                else
                {
                    Debug.Log("Client disconnected from the server");
                    connection = default(NetworkConnection);
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

            msg.ReceivedOnClient();
        }

        public virtual void SendToServer(NetMessage msg)
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