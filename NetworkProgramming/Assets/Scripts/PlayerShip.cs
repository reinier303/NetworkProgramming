using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkProgramming
{
    public class PlayerShip : BaseEntity
    {
        //References
        private GameManager gameManager;
        private ObjectPooler objectPooler;
        private ActorManager actorManager;

        //Movement
        public float BaseSpeed;

        //Networking
        BaseClient client;

        //Firing
        public float Damage, BulletSpeed;
        public float FireRate;
        private bool canFire;
        public ParticleSystem MuzzleFlash;
        public Image RedVignette;

        protected override void Awake()
        {
            base.Awake();
            client = GetComponent<BaseClient>();
            MuzzleFlash = transform.GetChild(0).GetComponent<ParticleSystem>();
            RedVignette.color = new Color(RedVignette.color.r, RedVignette.color.g, RedVignette.color.b, 0);
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            objectPooler = ObjectPooler.Instance;
            actorManager = ActorManager.Instance;

            canFire = true;
        }


        // Update is called once per frame
        private void Update()
        {
            if (!gameManager.Synced) { return; }
            if (PlayerId != gameManager.PlayerId)
            {
                MoveFromActorManager();
            }
            else
            {
                //Movement
                float axisValue = Input.GetAxis("Horizontal");
                if (axisValue != 0 && CheckEdges())
                {
                    Move(axisValue);
                }
                //Firing
                if(Input.GetMouseButton(0) && canFire)
                {
                    StartCoroutine(Fire());
                }

            }
        }

        private void Move(float axisValue)
        {
            float translation = axisValue * Time.deltaTime * BaseSpeed;

            SendChangePosition(new Vector2(transform.position.x + translation, transform.position.y));
        }

        private void MoveFromActorManager()
        {
            transform.position = actorManager.GetPlayerPosition(PlayerId);
        }

        private bool CheckEdges()
        {
            if (transform.position.x > 8.4f)
            {
                SendChangePosition(new Vector2(8.399f, transform.position.y));
                return false;
            }
            if (transform.position.x < -8.4f)
            {
                SendChangePosition(new Vector2(-8.399f, transform.position.y));
                return false;
            }
            return true;
        }

        private void SendChangePosition(Vector2 position)
        {
            transform.position = position;

            Net_PlayerPosition ps = new Net_PlayerPosition(gameManager.PlayerId, position.x, position.y);
            client.SendToServer(ps);
        }

        private IEnumerator Fire()
        {
            canFire = false;

            //Calculate
            Vector3 position = transform.position + new Vector3(0, transform.position.y < 0 ? 0.3f : -0.3f, 0);
            float zRotation = PlayerId == 0 ? 0 : 180;
            //Send to server
            Net_BulletData bd = new Net_BulletData(gameManager.PlayerId, position.x, position.y, zRotation, Damage, BulletSpeed);
            client.SendToServer(bd);

            //Spawn local
            PlayerBullet bullet = objectPooler.SpawnFromPool("PlayerBullet", position, transform.rotation).GetComponent<PlayerBullet>();
            MuzzleFlash.Play();
            bullet.Initialize(PlayerId, Damage, BulletSpeed);
            yield return new WaitForSeconds(1 / FireRate);
            canFire = true;
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerBullet bullet = collision.GetComponent<PlayerBullet>();

            if (bullet != null && bullet.PlayerId != PlayerId)
            {
                healthComponent.TakeDamage(bullet);
                if(gameManager.PlayerId != bullet.PlayerId)
                {
                    StartCoroutine(HitVignette());
                }
                bullet.Die();
            }
        }

        private IEnumerator HitVignette()
        {
            float r = RedVignette.color.r;
            float g = RedVignette.color.g;
            float b = RedVignette.color.b;
            float a = 0;

            float timeElapsed = 0;
            float lerpDuration = 0.1f;

            while (timeElapsed < lerpDuration)
            {
                a = Mathf.Lerp(0, 0.5f, timeElapsed / lerpDuration);
                RedVignette.color = new Color(r,g,b,a);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            timeElapsed = 0;
            while (timeElapsed < lerpDuration)
            {
                a = Mathf.Lerp(0.5f, 0f, timeElapsed / lerpDuration);
                RedVignette.color = new Color(r, g, b, a);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            a = 0;
            RedVignette.color = new Color(r, g, b, a);

        }
    }

}