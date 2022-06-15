using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace NetworkProgramming
{
    public class PowerupCard : MonoBehaviour
    {
        private GameManager gameManager;
        private PowerupManager powerupManager;

        [SerializeField] private Powerups powerup;
        [SerializeField] private float Value;

        [SerializeField] private Transform poolParent;

        [SerializeField] private TextMeshProUGUI cardText;

        private void Start()
        {
            gameManager = GameManager.Instance;
            powerupManager = PowerupManager.Instance;
            poolParent = transform.parent;
        }

        public void InitializeCard(Powerups powerup)
        {
            this.powerup = powerup;
            cardText.text = "+" + Value + " " + powerup.ToString();
        }

        public void DisableCard()
        {
            transform.SetParent(poolParent);
            gameObject.SetActive(false);
        }

        public void GrantPowerup()
        {
            if(gameManager.PlayerId != powerupManager.PickTurn) { return; }

            powerupManager.ChangeTurn(powerupManager.PickTurn == 0 ? 1 : 0);
            //Net_PickTurn turn = new Net_PickTurn(powerupManager.PickTurn);
            //gameManager.ThisClient.SendToServer(turn);          

            int siblingIndex = transform.GetSiblingIndex();
            powerupManager.PowerupSelected(siblingIndex);
            Net_PowerupSelection pt = new Net_PowerupSelection(gameManager.PlayerId, siblingIndex);
            gameManager.ThisClient.SendToServer(pt);

            PlayerShip ship = gameManager.GetPlayerShip(gameManager.PlayerId);
            switch(powerup)
            {
                case Powerups.Damage: ship.Damage += Value; break;
                case Powerups.FireRate: ship.FireRate += Value; break;
                case Powerups.BulletSpeed: ship.BulletSpeed += Value; break;
                case Powerups.MovementSpeed: ship.BaseSpeed += Value; break;
                default: break;
            }
        }
    }

    public enum Powerups
    {
        Damage,
        FireRate,
        BulletSpeed,
        MovementSpeed
    }
}
