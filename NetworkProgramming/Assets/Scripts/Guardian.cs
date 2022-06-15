using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkProgramming
{
    public class Guardian : BaseEntity
    {
        private GameManager gameManager;

        //Movement
        public float MoveSpeed = 1;
        public float MoveAmount = 3;
        private int direction;
        private Vector2 startPosition;
        //private bool syncDone;

        private bool dodging;

        float counter;

        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.Instance;
            //syncDone = false;
            startPosition = transform.position;
            direction = transform.position.y < 0 ? -1 : 1;
            PlayerId = transform.position.y < 0 ? 0 : 1;
        }

        // Update is called once per frame
        void Update()
        {
            if(gameManager.Synced)
            {
                counter += Time.deltaTime;
                transform.position = startPosition + new Vector2(Mathf.Sin(counter * MoveSpeed) * 3 * direction, 0.0f);
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerBullet bullet = collision.GetComponent<PlayerBullet>();
            if (bullet != null)
            {
                if (bullet.PlayerId == PlayerId)
                {
                    if (!dodging) { StartCoroutine(DodgeAnimation()); }
                }
                else
                {
                    healthComponent.TakeDamage(bullet);
                    bullet.Die();
                }
            }
        }

        private IEnumerator DodgeAnimation()
        {
            dodging = true;
            float timeElapsed = 0;
            float lerpDuration = 0.5f;
            float z = direction < 0 ? 0 : 180;

            while (timeElapsed < lerpDuration)
            {
                transform.localRotation = Quaternion.Euler(0, Mathf.Lerp(0, 360, timeElapsed / lerpDuration), z);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            transform.localRotation = Quaternion.Euler(0, 0, z);
            dodging = false;
        }
    }

}