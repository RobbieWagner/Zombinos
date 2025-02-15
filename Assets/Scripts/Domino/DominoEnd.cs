using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.Zombinos
{
    public enum DominoAbility
    {
        //TODO: Implement
    }

    public class DominoEnd : MonoBehaviour
    {
        [SerializeField] private Image visual;
        [SerializeField] private int pips = 0;
        public virtual int Pips
        { 
            get 
            { 
                return pips; 
            } 
            set 
            {
                if (pips == value)
                    return;

                pips = value;
                Math.Clamp(pips, 0, 6);
                
                OnPipValueChanged?.Invoke(pips);

                UpdateVisual();
            }
        }
        public Action<int> OnPipValueChanged = (int pipValue) => { };

        private void UpdateVisual()
        {
            if(pips > 0)
            {
                visual.enabled = true;
                visual.sprite = DominoManager.Instance.GetDominoPipSprite(pips);
            }
            else
                visual.enabled = false;
        }
    }
}