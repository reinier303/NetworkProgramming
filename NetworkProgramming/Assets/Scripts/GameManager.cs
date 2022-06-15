using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace NetworkProgramming
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        private PowerupManager powerupManager;
        private HighscoreManager highscoreManager;

        public ASGClient ThisClient;
        public string Message;
        public int PlayerId;

        public List<PlayerShip> Ships;

        //Powerups
        [SerializeField] private Slider powerUpTimer1, powerUpTimer2;
        public float PowerUpTimeMax;
        private float currentPowerUpTime;
        public int PowerupAmount = 3;

        public bool Synced;

        public TextMeshProUGUI WinText;

        public float matchLength;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            powerupManager = PowerupManager.Instance;
            highscoreManager = HighscoreManager.Instance;
            InitializePowerupTimers();
        }

        private void InitializePowerupTimers()
        {
            currentPowerUpTime = 0;
            powerUpTimer1.maxValue = PowerUpTimeMax;
            powerUpTimer1.value = 0;
            powerUpTimer2.maxValue = PowerUpTimeMax;
            powerUpTimer2.value = 0;
        }

        private void UpdatePowerupTimer()
        {
            currentPowerUpTime += Time.deltaTime;
            powerUpTimer1.value = currentPowerUpTime;
            powerUpTimer2.value = currentPowerUpTime;

            if(currentPowerUpTime >= PowerUpTimeMax)
            {
                OpenPowerUpSelection();
                currentPowerUpTime = 0;
            }

            matchLength += Time.deltaTime;
        }

        private void OpenPowerUpSelection()
        {
            Time.timeScale = 0;
            powerupManager.ShowPowerups(PowerupAmount);
        }

        public void SetPlayerId(int id)
        {
            PlayerId = id;
            Ships[0].gameObject.SetActive(true);
            Ships[1].gameObject.SetActive(true);
            if(id == 1)
            {
                ASGClient client1 = Ships[1].GetComponent<ASGClient>();
                client1.enabled = true;
                Ships[0].GetComponent<ASGClient>().enabled = false;
                ThisClient = client1;
            } else {
                ASGClient client0 = Ships[0].GetComponent<ASGClient>();
                client0.enabled = true;
                Ships[1].GetComponent<ASGClient>().enabled = false;
                ThisClient = client0;
            }
        }

        public PlayerShip GetPlayerShip(int id)
        {
            return Ships[id];
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                //TODO::TESTING CODE DONT FORGET TO REMOVE
                Net_ChatMessage msg = new Net_ChatMessage(Message);
                FindObjectOfType<BaseClient>().SendToServer(msg);
            }      
            if (Synced) { UpdatePowerupTimer(); }
        }

        public void PlayerWin(int id)
        {
            if(id == 0) { id = 1; } else { id = 0; }
            WinText.gameObject.SetActive(true);
            WinText.text = "PLAYER " + id + " WINS!!!";
            highscoreManager.UpdateHighscore();
        }
    }
}