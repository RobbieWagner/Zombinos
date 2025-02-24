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
                    CombatManager.Instance?.DiscardDomino(domino);

                domino = value;
                OnDominoSet?.Invoke(domino, this);
                if (domino != null)
                    OnSetDomino();
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
        }

        private void OnSetDomino()
        {
            Domino.transform.SetParent(this.transform);
            Domino.transform.localPosition = new Vector3(0, 0, -1);
            Domino.button.interactable = false;
        }

        private void TryPlaceDomino()
        {
            if (CombatManager.Instance.ConfirmedDomino != null && CombatManager.Instance.CurrentCombatPhase == CombatPhase.PLAYER)
                Domino = CombatManager.Instance.ConfirmedDomino;
        }
    }
}