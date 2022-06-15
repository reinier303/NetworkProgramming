using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkProgramming
{
    public class ActorManager : MonoBehaviour
    {
        public static ActorManager Instance;
        private GameManager gameManager;

        [SerializeField]private List<Vector2> playerPositions;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            playerPositions = new List<Vector2>();
            for (int i = 0; i < gameManager.Ships.Count; i++)
            {
                playerPositions.Add(gameManager.Ships[i].transform.position);
            }
        }

        public Vector2 GetPlayerPosition(int playerId)
        {
            return playerPositions[playerId];
        }

        public void SetPlayerPosition(int playerId, Vector2 position)
        {
            playerPositions[playerId] = position;
        }
    }
}
