using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace NetworkProgramming
{
    public class AnotherSpaceGameServer : BaseServer
    {
        private GameManager gameManager;

        bool initialSync;

        private void Awake()
        {
            initialSync = true;
        }

        public override void Init()
        {
            base.Init();
            gameManager = GameManager.Instance;
        }

        protected override void AcceptNewConnection()
        {
            base.AcceptNewConnection();
            if(connections.Length > 1 && initialSync)
            {
                NetMessage msg = new Net_SyncDone(1, 1);
                Debug.LogError("Synced");
                Broadcast(msg);
                initialSync = false;
            }
        }

        public override void OnData(DataStreamReader stream)
        {
            NetMessage msg = null;

            //Read first byte from data stream to uncover type
            var opCode = (OpCode)stream.ReadByte();

            switch (opCode)
            {
                case OpCode.CHAT_MESSAGE: msg = new Net_ChatMessage(stream); break;
                case OpCode.PLAYER_POSITION: 
                    msg = new Net_PlayerPosition(stream);
                    //Send position to all players                 
                    break;
                case OpCode.BULLET_DATA: msg = new Net_BulletData(stream); 
                    break;
                case OpCode.SYNC_DONE:
                    msg = new Net_SyncDone(stream);
                    break;
                case OpCode.POWERUP_TYPE: msg = new Net_PowerupType(stream); break;
                case OpCode.POWERUP_SELECTION: msg = new Net_PowerupSelection(stream); break;
                case OpCode.PICK_TURN: msg = new Net_PickTurn(stream); break;
                default: Debug.Log("Message received without OpCode"); break;
            }
            //Broadcast(msg);


            msg.ReceivedOnServer(this);
        }
    }
}
