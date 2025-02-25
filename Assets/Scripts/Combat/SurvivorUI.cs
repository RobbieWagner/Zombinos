using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.Zombinos
{
    public class SurvivorUI : MonoBehaviour
    {
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private SpriteRenderer survivorSprite;

        private Survivor survivor = null;
        public Survivor Survivor
        { 
            get 
            { 
                return survivor; 
            }
            set 
            {
                if (survivor == value)
                    return;

                if (survivor != null)
                    survivor.OnUpdateHealth -= UpdateHPUI;
                
                survivor = value;

                if (survivor != null)
                {
                    survivor.OnUpdateHealth += UpdateHPUI;
                    UpdateHPUI(survivor.HP);
                    UpdateUI();
                }
            }
        }
       
        private void UpdateUI()
        {
            if (survivor.survivorInfo != null)
                survivorSprite.sprite = Resources.Load<Sprite>(StaticGameStats.survivorSpritesFilePath + survivor.survivorInfo.survivorSpritePath);
        }

        private void UpdateHPUI(int hp)
        {
            hpSlider.maxValue = survivor.survivorInfo.maxHP;
            hpSlider.value = hp;
            hpText.text = $"{hp}/{survivor.survivorInfo.maxHP}";
        }
    }
}