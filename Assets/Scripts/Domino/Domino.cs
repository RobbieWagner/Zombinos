using RobbieWagnerGames.Managers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RobbieWagnerGames.Zombinos
{
    public class Domino : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Domino")]
        public ButtonListener button;
        private bool canSelect = true;

        public SpriteRenderer offenseEndImage;
        private int offenseCurrentStrength = 0;
        public int OffenseCurrentStrength 
        {
            get
            {
                return offenseCurrentStrength;
            }
            set
            {
                if (offenseCurrentStrength == value)
                    return;
                offenseCurrentStrength = value;
                offenseEndImage.sprite = DominoManager.GetDominoPipSprite(offenseCurrentStrength);
            }
        }
        public SpriteRenderer defenseEndImage;
        private int defenseCurrentStrength = 0;
        public int DefenseCurrentStrength 
        { 
            get
            {
                return defenseCurrentStrength;
            }
            set
            {
                if (defenseCurrentStrength == value)
                    return;
                defenseCurrentStrength = value;
                defenseEndImage.sprite = DominoManager.GetDominoPipSprite(defenseCurrentStrength);
            }
        }
        public Vector3 defaultScale;

        private DominoConfiguration dominoConfiguration;
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
                CombatManager.Instance.SelectedDomino = this;
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
            //Debug.Log("pointer enter");
            if(button.interactable & canSelect)
                EventSystemManager.Instance.SetSelectedGameObject(button.gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(EventSystemManager.Instance.eventSystem.currentSelectedGameObject == button.gameObject)
                EventSystemManager.Instance.SetSelectedGameObject(null);
        }

        public IEnumerator CooldownMouseHover(float timeToWait = .4f)
        {
            canSelect = false;
            yield return new WaitForSeconds(timeToWait);

            canSelect = true;
        }
    }
}