using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.Zombinos
{
    public enum CombatPhase
    {
        NONE,
        SETUP,
        TURN_START,
        PLAYER,
        EXECUTION,
        TURN_END,
        WIN,
        LOSE
    }

    public partial class CombatManager : MonoBehaviourSingleton<CombatManager>
    {
        [Header("Dominos")]
        [SerializeField] private Domino playerDominoPrefab;
        [SerializeField] private Domino enemyDominoPrefab;
        [SerializeField] private List<DominoConfiguration> baseDeck;
        private List<DominoConfiguration> currentDeck = new List<DominoConfiguration>();
        private List<Domino> playerHand;
        [SerializeField] private int playerHandSize = 5;

        [Header("Game Board")]
        [SerializeField] private List<DominoSpace> zombieDominoSpaces;
        [SerializeField] private List<DominoSpace> survivorDominoSpaces;
        [SerializeField] private List<SurvivorUI> survivorUIs;
        [SerializeField] private List<Survivor> currentSurvivors;
        [SerializeField] private int hordeCount;
        public int HordeCount
        {
            get
            {
                return hordeCount;
            }
            set
            {
                if (hordeCount == value)
                    return;

                hordeCount = value;
                OnModifyHordeCount?.Invoke(hordeCount);
            }
        }
        public Action<int> OnModifyHordeCount = (int newHordeCount) => { };

        private Coroutine phaseChangeCoroutine = null;
        private CombatPhase currentCombatPhase = CombatPhase.NONE;
        public CombatPhase CurrentCombatPhase
        {
            get
            {
                return currentCombatPhase;
            }
            set
            {
                if (currentCombatPhase == value || phaseChangeCoroutine != null)
                    return;

                phaseChangeCoroutine = StartCoroutine(SetCombatPhaseCo(value));
            }
        }
        public Action<CombatPhase> OnCombatPhaseChange = (CombatPhase value) => { };

        public IEnumerator SetCombatPhaseCo(CombatPhase phase)
        {
            // First, end the current phase of combat (Check for events that trigger)
            yield return StartCoroutine(EndCurrentPhase());

            // switch the phases
            currentCombatPhase = phase;
            OnCombatPhaseChange?.Invoke(currentCombatPhase);

            phaseChangeCoroutine = null;
        }

        protected virtual IEnumerator EndCurrentPhase()
        {
            switch (currentCombatPhase)
            {
                case CombatPhase.SETUP:
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.SETUP_COMPLETE));
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.COMBAT_STARTED));
                    break;
                case CombatPhase.PLAYER:
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.PLAYER_PHASE_ENDED));
                    break;
                case CombatPhase.EXECUTION:
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.EXECUTION_PHASE_ENDED));
                    break;
                case CombatPhase.WIN:
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.COMBAT_TERMINATED));
                    break;
                case CombatPhase.LOSE:
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.COMBAT_TERMINATED));
                    break;
                case CombatPhase.NONE:
                    break;
                default:
                    throw new NotImplementedException($"Functionality for combatInfo phase {currentCombatPhase} is not implemented for the current kind of combatInfo!!");
            }
        }

        public void StartCombat()
        {
            CurrentCombatPhase = CombatPhase.SETUP;
        }
    }
}