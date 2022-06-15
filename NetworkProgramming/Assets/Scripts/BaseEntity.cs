using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkProgramming
{
    public class BaseEntity : MonoBehaviour
    {
        public int PlayerId;

        //Health
        protected HealthComponent healthComponent;

        protected virtual void Awake()
        {
            healthComponent = GetComponent<HealthComponent>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerBullet bullet = collision.GetComponent<PlayerBullet>();

            if (bullet != null && bullet.PlayerId != PlayerId)
            {
                healthComponent.TakeDamage(bullet);
                bullet.Die();
            }
        }
    }
}
