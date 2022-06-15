using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkProgramming
{
    public class HealthComponent : MonoBehaviour
    {
        public bool IsPlayer;
        public float MaxHealth;
        private float currentHealth;
        private GameManager gameManager;
        private ObjectPooler objectPooler;

        private void Start()
        {
            currentHealth = MaxHealth;
            objectPooler = ObjectPooler.Instance;
            gameManager = GameManager.Instance;
        }

        private void OnEnable()
        {
            currentHealth = MaxHealth;
        }

        public void TakeDamage(PlayerBullet bullet)
        {
            currentHealth -= bullet.Damage;
            if (currentHealth < 1)
            {
                if (IsPlayer) { PlayerDie(); }
                else { Die(); }
            }
        }

        private void Die()
        {
            objectPooler.SpawnFromPool("Explosion", transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }

        private void PlayerDie()
        {
            objectPooler.SpawnFromPool("Explosion", transform.position, Quaternion.identity);
            gameManager.PlayerWin(GetComponent<PlayerShip>().PlayerId);
            gameObject.SetActive(false);
            //SEND PLAYER WIN
            //CHECK IF BOTH PLAYERS ARE ON DIFFERENT ACCOUNT... IF NOT DONT SEND HIGHSCORE
        }
    }
}
