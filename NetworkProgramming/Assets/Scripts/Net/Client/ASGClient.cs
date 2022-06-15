using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace NetworkProgramming
{
    public class ASGClient : BaseClient
    {
        public ActorManager actorManager;
        private ObjectPooler objectPooler;
        private GameManager gameManager;
        private PowerupManager powerupManager;
        private PlayerShip otherShip;


        public override void Init()
        {
            base.Init();
            actorManager = ActorManager.Instance;
            objectPooler = ObjectPooler.Instance;
            gameManager = GameManager.Instance;
            powerupManager = PowerupManager.Instance;
            int otherShipId = gameManager.PlayerId == 0 ? 1 : 0;
            otherShip = gameManager.Ships[otherShipId];
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
                    SetPlayerPosition(stream);
                    break;
                case OpCode.BULLET_DATA: msg = new Net_BulletData(stream);
                    SpawnBullet(stream);
                    break;
                case OpCode.POWERUP_TYPE:
                    msg = new Net_PowerupType(stream);
                    if (gameManager.PlayerId == 0)
                    {
                        return;
                    }
                    int amount = stream.ReadInt();
                    for (int i = 0; i < amount; i++)
                    {
                        powerupManager.currentPowerups.Add(stream.ReadInt());
                    }
                    break;
                case OpCode.POWERUP_SELECTION:
                    msg = new Net_PowerupSelection(stream);
                    SelectPowerup(stream);
                    break;
                case OpCode.SYNC_DONE: msg = new Net_SyncDone(stream);
                    SyncDone(stream);
                    break;
                case OpCode.PICK_TURN:
                    msg = new Net_PickTurn(stream);
                    if (gameManager.PlayerId == 0){ return; }
                    int turn = stream.ReadInt();
                    powerupManager.ChangeTurn(turn);
                    break;

                default: Debug.Log("Message received without OpCode"); break;
            }
            //USE FOR DEBUGGING
            //msg.ReceivedOnClient();
        }

        private void SetPlayerPosition(DataStreamReader stream)
        {
            int playerId = stream.ReadInt();
            float positionX = stream.ReadFloat();
            float positionY = stream.ReadFloat();
            Vector2 playerPosition = new Vector2(positionX, positionY);
            actorManager.SetPlayerPosition(playerId, playerPosition);
        }

        private void SpawnBullet(DataStreamReader stream)
        {
            int playerId = stream.ReadInt();
            if (gameManager.PlayerId == playerId)
            {
                return;
            }
            float positionX = stream.ReadFloat();
            float positionY = stream.ReadFloat();
            Vector2 position = new Vector2(positionX, positionY);
            float rotationZ = stream.ReadFloat();
            float damage = stream.ReadFloat();
            float speed = stream.ReadFloat();
            PlayerBullet bullet = objectPooler.SpawnFromPool("PlayerBullet", position, Quaternion.Euler(0,0,rotationZ)).GetComponent<PlayerBullet>();

            otherShip.MuzzleFlash.Play();

            bullet.Initialize(playerId , damage, speed);
        }

        private void SelectPowerup(DataStreamReader stream)
        {
            int playerId = stream.ReadInt();
            int selection = stream.ReadInt();
            if(playerId != gameManager.PlayerId) 
            { 
                powerupManager.PowerupSelected(selection);
                powerupManager.ChangeTurn(powerupManager.PickTurn == 0 ? 1 : 0);
            }
        }

        private void SyncDone(DataStreamReader stream)
        {
            int syncDone = stream.ReadInt();
            int initialSync = stream.ReadInt();
            if (syncDone == 1)
            {
                gameManager.Synced = true;
                if (initialSync == 1 && gameManager.PlayerId == 0) { powerupManager.StartTurn(); }
            }
        }
    }
}
