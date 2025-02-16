using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.Zombinos
{
    public class CombatEvent : EventSequence
    {
        [Space(15)]
        [Header("Settings")]
        public int priority = -1;
        [SerializeField] private CombatEventTriggerType eventTrigger;
        [SerializeField] private bool triggersOnce = true;

        protected virtual void Awake()
        {
            StartCoroutine(SubscribeCombatEvent());
        }

        public virtual IEnumerator SubscribeCombatEvent()
        {
            //yields to make sure subscription happens if combat manager has not been established
            yield return null;
            if (CombatManager.Instance != null)
                CombatManager.Instance.SubscribeEventToCombatEventHandler(this, eventTrigger);
        }

        protected virtual void UnsubscribeCombatEvent()
        {
            if (CombatManager.Instance != null)
                CombatManager.Instance.UnsubscribeEventFromCombatEventHandler(this, eventTrigger);
        }

        public virtual IEnumerator InvokeCombatEvent()
        {
            yield return StartCoroutine(InvokeEvent());
        }

        public override IEnumerator InvokeEvent(bool setToEventGameMode = true)
        {
            yield return StartCoroutine(base.InvokeEvent(setToEventGameMode));
            if (triggersOnce) UnsubscribeCombatEvent();
        }
    }
}