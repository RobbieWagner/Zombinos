using RobbieWagnerGames.Managers;
using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        private List<DominoConfiguration> discard = new List<DominoConfiguration>();
        private List<Domino> playerHand = new List<Domino>();
        [SerializeField] private Transform playerHandParent;
        [SerializeField] private Transform handTransformPrefab;
        private List<Transform> handTransforms = new List<Transform>();
        [SerializeField] private int playerHandSize = 5;

        [Header("Game Board")]
        [SerializeField] private List<DominoSpace> zombieDominoSpaces;
        [SerializeField] private List<DominoSpace> survivorDominoSpaces;
        [SerializeField] private List<SurvivorUI> survivorUis;
        private List<Survivor> currentSurvivors = new List<Survivor>();
        [SerializeField] private List<DominoConfiguration> hordeDominoOptions;
        [SerializeField] private TextMeshProUGUI hordeText;
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

                hordeCount = Math.Clamp(value, 0, 999);
                OnModifyHordeCount?.Invoke(hordeCount);
                hordeText.text = $"{hordeCount}";
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

        protected override void Awake()
        {
            base.Awake();
            SetupCombatEventHandlers();
            OnCombatPhaseChange += StartCombatPhase;

            hordeText.text = $"{hordeCount}";

            StartCombat();

            foreach(DominoSpace space in survivorDominoSpaces)
                space.OnDominoSet += PlaceDomino;

            InputManager.Instance.gameControls.UI.Navigate.performed += CheckNullNavigation;
        }

        protected virtual void StartCombatPhase(CombatPhase phase)
        {
            switch (phase)
            {
                case CombatPhase.SETUP:
                    StartCoroutine(WaitForCombatEvents(CombatEventTriggerType.SETUP_STARTED, () => { StartCoroutine(SetupCombat()); }));
                    break;
                case CombatPhase.TURN_START:
                    StartCoroutine(WaitForCombatEvents(CombatEventTriggerType.TURN_STARTED, () => { StartCoroutine(StartTurn()); }));
                    break;
                case CombatPhase.PLAYER:
                    StartCoroutine(WaitForCombatEvents(CombatEventTriggerType.PLAYER_PHASE_STARTED, () => { StartCoroutine(HandlePlayerPhase()); }));
                    break;
                case CombatPhase.EXECUTION:
                    StartCoroutine(WaitForCombatEvents(CombatEventTriggerType.EXECUTION_PHASE_STARTED, () => { StartCoroutine(HandleExecutionPhase()); }));
                    break;
                case CombatPhase.TURN_END:
                    StartCoroutine(WaitForCombatEvents(CombatEventTriggerType.TURN_ENDED, () => { StartCoroutine(EndTurn()); }));
                    break;
                case CombatPhase.WIN:
                    StartCoroutine(WaitForCombatEvents(CombatEventTriggerType.COMBAT_WON, () => { StartCoroutine(ResolveCombat(true)); }));
                    break;
                case CombatPhase.LOSE:
                    StartCoroutine(WaitForCombatEvents(CombatEventTriggerType.COMBAT_LOST, () => { StartCoroutine(ResolveCombat(false)); }));
                    break;
                case CombatPhase.NONE:
                    break;
                default:
                    throw new ArgumentException($"Invalid combatInfo phase used {phase}");
            }
        }

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
                case CombatPhase.TURN_START:
                    break;
                case CombatPhase.PLAYER:
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.PLAYER_PHASE_ENDED));
                    break;
                case CombatPhase.EXECUTION:
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.EXECUTION_PHASE_ENDED));
                    break;
                case CombatPhase.TURN_END:
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

        public void DiscardDomino(Domino domino)
        {
            discard.Add(domino.DominoConfiguration);
            Destroy(domino.gameObject);
        }
    }
}