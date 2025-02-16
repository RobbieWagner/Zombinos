using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RobbieWagnerGames.Zombinos
{
    public class DominoSpace : Selectable
    {
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
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);  
            Debug.Log(this.gameObject.name + " was selected");
        }

        private void OnSetDomino()
        {
            Domino.transform.position = new Vector3(transform.position.x, transform.position.y, Domino.transform.position.z);
        }
    }
}