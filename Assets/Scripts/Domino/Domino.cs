using RobbieWagnerGames.Managers;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RobbieWagnerGames.Zombinos
{
    public class Domino : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Domino")]
        public ButtonListener button;
        public SpriteRenderer offenseEndImage;
        private int offenseCurrentStrength;
        public SpriteRenderer defenseEndImage;
        private int defenseCurrentStrength;
        public Vector3 defaultScale;

        public DominoConfiguration dominoConfiguration;
        public DominoConfiguration DominoConfiguration
        {
            get
            {
                return dominoConfiguration;
            }
            set
            {
                if (dominoConfiguration == value)
                    return;
                dominoConfiguration = value;
                UpdateDomino();
            }
        }

        protected void Awake()
        {
            button.onClick.AddListener(OnConfirmDomino);
            button.OnButtonSelected += OnSelect;
            button.OnButtonDeselected += OnDeselect;

            transform.localScale = defaultScale;
        }

        private void UpdateDomino()
        {
            if (dominoConfiguration.offenseEndType != DominoAttributeType.NONE)
                offenseEndImage.sprite = DominoManager.GetDominoAttributeSprite(dominoConfiguration.offenseEndType);
            else 
                offenseEndImage.sprite = DominoManager.GetDominoPipSprite(dominoConfiguration.offenseEndStrength);

            if (dominoConfiguration.defenseEndType != DominoAttributeType.NONE)
                defenseEndImage.sprite = DominoManager.GetDominoAttributeSprite(dominoConfiguration.defenseEndType);
            else
                defenseEndImage.sprite = DominoManager.GetDominoPipSprite(dominoConfiguration.defenseEndStrength);

            offenseCurrentStrength = dominoConfiguration.offenseEndStrength;
            defenseCurrentStrength = dominoConfiguration.defenseEndStrength;
        }

        public void OnSelect(Button button)
        {
            if (button.interactable)
            {
                CombatManager.Instance.SelectedDomino = this;
                Debug.Log($"Domino selected {offenseCurrentStrength}/{defenseCurrentStrength}");
            }
        }

        public void OnDeselect(Button button)
        {
            if (CombatManager.Instance.SelectedDomino == this)
                CombatManager.Instance.SelectedDomino = null;
        }

        private void OnConfirmDomino()
        {
            if(button.interactable)
                CombatManager.Instance.ConfirmedDomino = this;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("pointer enter");
            EventSystemManager.Instance.SetSelectedGameObject(button.gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(EventSystemManager.Instance.eventSystem.currentSelectedGameObject == button.gameObject)
                EventSystemManager.Instance.SetSelectedGameObject(null);
        }
    }
}