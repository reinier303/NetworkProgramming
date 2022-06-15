using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace NetworkProgramming
{
    public class PowerupManager : MonoBehaviour
    {
        public static PowerupManager Instance;

        private GameManager gameManager;
        private ObjectPooler objectPooler;

        [SerializeField] private Transform cardHolder;

        [SerializeField] private int poolSize;

        private List<int> powerupPool;

        public List<int> currentPowerups;

        public int SelectedPowerups;

        public int PickTurn;
        int firstTurn;

        public int TurnsLocal;

        public TextMeshProUGUI PowerupText;

        private void Awake()
        {
            PickTurn = 2;
            Instance = this;
            PowerupText.gameObject.SetActive(false);
            firstTurn = 0;
            TurnsLocal = 0;
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            objectPooler = ObjectPooler.Instance;
            currentPowerups = new List<int>();
            if (gameManager.PlayerId == 0)
            {
                GeneratePowerupPool();
            }
        }

        public void ChangeTurn(int turn)
        {
            //Use for initialisation
            if (PickTurn == 2)
            {
                PickTurn = turn;
                AdjustPowerupText(false);
                return;
            }
            if (firstTurn < 1)
            {
                firstTurn++;
                PickTurn = turn;
            }
            else { 
                firstTurn = 0;
            }

            AdjustPowerupText();         
        }

        public void AdjustPowerupText(bool enable = true)
        {
            PowerupText.gameObject.SetActive(enable);
            PowerupText.text = "Player " + PickTurn + " Choose a Powerup";
        }

        public void StartTurn()
        {
            PickTurn = Random.Range(0, 2);
            Net_PickTurn turn = new Net_PickTurn(PickTurn);
            gameManager.ThisClient.SendToServer(turn);
        }

        private void GeneratePowerupPool()
        {
            int powerUpTypeAmount = Powerups.GetNames(typeof(Powerups)).Length;
            powerupPool = new List<int>();
            for (int i = 0; i < poolSize; i++)
            {
                powerupPool.Add(Random.Range(0, powerUpTypeAmount));
            }
        }

        public void ShowPowerups(int amount)
        {
            if(gameManager.PlayerId == 0)
            {
                for (int i = 0; i < amount; i++)
                {
                    currentPowerups.Add(powerupPool[Random.Range(0, powerupPool.Count)]);
                }
                Net_PowerupType pt = new Net_PowerupType(amount, currentPowerups);
                gameManager.ThisClient.SendToServer(pt);
            }
            StartCoroutine(WaitForCards(amount));
            AdjustPowerupText();
        }

        private IEnumerator WaitForCards(float amount)
        {
            while(currentPowerups.Count < amount)
            {
                yield return new WaitForEndOfFrame();
            }
            for (int i = 0; i < amount; i++)
            {
                GameObject card = objectPooler.SpawnFromPool("PowerupCard", Vector2.zero, Quaternion.identity);
                card.transform.SetParent(cardHolder);
                card.GetComponent<PowerupCard>().InitializeCard((Powerups)currentPowerups[i]);
            }
            gameManager.Synced = false;
        }

        public void PowerupSelected(int selection)
        {
            cardHolder.GetChild(selection).gameObject.SetActive(false);
            SelectedPowerups++;
            if(SelectedPowerups > 1)
            {
                DisableAllCards();
            }
        }

        public void DisableAllCards()
        {
            SelectedPowerups = 0;
            Time.timeScale = 1;
            gameManager.Synced = true;
            foreach (PowerupCard card in transform.GetComponentsInChildren<PowerupCard>())
            {
                card.DisableCard();
            }
            currentPowerups.Clear();
            StartCoroutine(LazyVisualHotfixMightChange());
        }

        private IEnumerator LazyVisualHotfixMightChange()
        {
            yield return new WaitForSeconds(0.05f);
            PowerupText.gameObject.SetActive(false);
        }

    }
}
