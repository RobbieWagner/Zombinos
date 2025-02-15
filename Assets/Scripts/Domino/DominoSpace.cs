using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RobbieWagnerGames.Zombinos
{
    public class DominoSpace : Selectable
    {
        public Domino domino;

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);  
            Debug.Log(this.gameObject.name + " was selected");
        }
    }
}