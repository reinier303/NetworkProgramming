using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkProgramming
{
    public class PlayerBullet : MonoBehaviour
    {
        private ObjectPooler objectPooler;

        private float speed = 5;
        public float Damage = 1;

        public int PlayerId;

        private void Start()
        {
            objectPooler = ObjectPooler.Instance;
        }

        public void Initialize(int playerId, float damage, float speed)
        {
            this.Damage = damage;
            this.speed = speed;
            PlayerId = playerId;
        }

        public void Die()
        {
            objectPooler.SpawnFromPool("PlayerBulletExplosionYellow", transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.position += transform.up * speed * Time.deltaTime;
        }
    }
}