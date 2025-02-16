using RobbieWagnerGames.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RobbieWagnerGames.Zombinos
{
    public class DominoSpace : MonoBehaviour
    {
        public ButtonListener button;
        public bool isOnPlayersSide = false;

        private Domino domino;
        public Domino Domino
        {
            get 
            {
                return domino;
            }
            set 
            {
                if (domino == value)
                    return;

                if (domino != null)
                    Destroy(domino.gameObject);

                domino = value;
                OnSetDomino();
                OnDominoSet?.Invoke(domino, this);
            }
        }
        public Action<Domino, DominoSpace> OnDominoSet = (Domino domino, DominoSpace space) => { };

        protected void Awake()
        {
            if (isOnPlayersSide)
                button.onClick.AddListener(TryPlaceDomino);

            button.OnButtonSelected += OnSelect;
        }

        public void OnSelect(Button button)
        {
            Debug.Log(this.gameObject.name + " was selected");
        }

        private void OnSetDomino()
        {
            Domino.transform.parent = this.transform;
            Domino.transform.position = new Vector3(transform.position.x, transform.position.y, Domino.transform.position.z);
            Domino.button.interactable = false;
        }

        private void TryPlaceDomino()
        {
            if (CombatManager.Instance.ConfirmedDomino != null && CombatManager.Instance.CurrentCombatPhase == CombatPhase.PLAYER)
                Domino = CombatManager.Instance.ConfirmedDomino;
        }
    }
}