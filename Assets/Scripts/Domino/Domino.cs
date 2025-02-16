using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RobbieWagnerGames.Zombinos
{
    public class Domino : Selectable
    {
        [SerializeField] private SpriteRenderer offenseEndImage;
        private int offenseCurrentStrength;
        [SerializeField] private SpriteRenderer defenseEndImage;
        private int defenseCurrentStrength;

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
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
        }

    }
}