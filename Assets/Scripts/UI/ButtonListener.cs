using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RobbieWagnerGames.Zombinos
{
    public class ButtonListener : Button
    {
        public Action<Button> OnButtonSelected = (Button button) => { };
        public Action<Button> OnButtonDeselected = (Button button) => { };

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            OnButtonSelected?.Invoke(this);
        }

        public override void OnDeselect(BaseEventData eventData) 
        {
            base.OnDeselect(eventData);
            OnButtonDeselected?.Invoke(this);
        }
    }
}