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
                UpdateHPUI(survivor.HP);
                
                if (survivor != null) 
                    survivor.OnUpdateHealth += UpdateHPUI;
            }
        }
       

        private void UpdateHPUI(int hp)
        {
            hpSlider.maxValue = survivor.SurvivorInfo.maxHP;
            hpSlider.value = hp;
            hpText.text = $"{hp}/{survivor.SurvivorInfo.maxHP}";
        }
    }
}