using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using RobbieWagnerGames.Utilities;

namespace RobbieWagnerGames.Zombinos
{
    public enum CombatEventTriggerType
    {
        NONE = -1,

        SETUP_STARTED = 0,
        SETUP_COMPLETE = 1,

        COMBAT_STARTED = 2,

        PLAYER_PHASE_STARTED = 3,
        PLAYER_PHASE_ENDED = 4,

        EXECUTION_PHASE_STARTED = 5,
        EXECUTION_PHASE_ENDED = 6,

        COMBAT_WON = 7, // IF EVENT NEEDS TO TRIGGER NO MATTER HOW COMBAT ENDS, USE COMBAT TERMINATED/RESOLVED INSTEAD
        COMBAT_LOST = 8, // IF EVENT NEEDS TO TRIGGER NO MATTER HOW COMBAT ENDS, USE COMBAT TERMINATED/RESOLVED INSTEAD
        COMBAT_TERMINATED = 9,
    }

    /// <summary>
    /// Handles events that will interupt the flow of combat
    /// </summary>
    public partial class CombatManager : MonoBehaviourSingleton<CombatManager>
    {
        [Space(10)]
        [Header("Combat Events")]
        private Dictionary<CombatEventTriggerType, CombatEventHandler> combatEventHandlers = new Dictionary<CombatEventTriggerType, CombatEventHandler>();
        [SerializeField] private CombatEventHandler combatEventHandlerPrefab;
        [SerializeField] private Transform eventHandlerParent;
        [Space(10)]

        //private bool isInterrupted = false;
        private Coroutine currentInterruptionCoroutine;
        public delegate IEnumerator CombatCoroutineEventHandler();

        private void SetupCombatEventHandlers()
        {
            foreach (CombatEventTriggerType eventType in Enum.GetValues(typeof(CombatEventTriggerType)))
                combatEventHandlers.Add(eventType, Instantiate(combatEventHandlerPrefab, eventHandlerParent));
        }

        public void SubscribeEventToCombatEventHandler(CombatEvent combatEvent, CombatEventTriggerType triggerType)
        {
           // Debug.Log("attempt to subscribe");
            if (combatEventHandlers.Keys.Contains(triggerType))
                combatEventHandlers[triggerType].Subscribe(combatEvent, combatEvent.priority);
            else Debug.LogWarning($"Trigger type {triggerType} not found, please ensure that trigger type is valid for combatInfo event");
        }

        public void UnsubscribeEventFromCombatEventHandler(CombatEvent combatEvent, CombatEventTriggerType triggerType)
        {
            if (combatEventHandlers.Keys.Contains(triggerType))
                combatEventHandlers[triggerType].Unsubscribe(combatEvent);
            else Debug.LogWarning($"Trigger type {triggerType} not found, please ensure that trigger type is valid for combatInfo event");
        }

        public IEnumerator WaitForCombatEvents(CombatEventTriggerType triggerType, Action callback)
        {
            yield return StartCoroutine(RunCombatEvents(triggerType));
            callback?.Invoke();
        }

        public IEnumerator RunCombatEvents(CombatEventTriggerType triggerType)
        {
            // NOTE: FOR SOME REASON, ENUM IS OFF. ADDING 1 TO FIX THIS STRANGE ERROR, MAY NEED TO BE REVISITED
            if (combatEventHandlers.Keys.Contains(triggerType + 1))
                yield return StartCoroutine(combatEventHandlers[triggerType + 1].Invoke());
            
        }

        protected virtual IEnumerator InvokeCombatEvent(CombatCoroutineEventHandler handler, bool yield = true)
        {
            if (handler != null)
            {
                if (yield) foreach (CombatCoroutineEventHandler invocation in handler?.GetInvocationList()) yield return StartCoroutine(invocation?.Invoke());
                else foreach (CombatCoroutineEventHandler invocation in handler?.GetInvocationList()) StartCoroutine(invocation?.Invoke());
            }
        }
    }
}