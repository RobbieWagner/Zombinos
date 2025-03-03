using DG.Tweening;
using RobbieWagnerGames.Managers;
using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.Zombinos
{
    public enum DominoChainType
    {
        NONE,
        OFFENSE,
        DEFENSE
    }

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
        private List<DominoConfiguration> hordeDominoOptions;
        [SerializeField] private List<DominoConfiguration> baseDeck;
        private List<DominoConfiguration> currentDeck = new List<DominoConfiguration>();
        private List<DominoConfiguration> discard = new List<DominoConfiguration>();
        private List<Domino> playerHand = new List<Domino>();
        [SerializeField] private Transform playerHandParent;
        [SerializeField] private RectTransform handTransformPrefab;
        private List<Transform> handTransforms = new List<Transform>();
        [SerializeField] private int playerHandSize = 5;

        [Header("Game Board")]
        [SerializeField] private List<DominoSpace> zombieDominoSpaces;
        [SerializeField] private List<DominoSpace> survivorDominoSpaces;
        [SerializeField] private List<SurvivorUI> survivorUis;
        private List<Survivor> currentSurvivors = new List<Survivor>();
        [SerializeField] private TextMeshProUGUI hordeText;

        [Header("Combat")]
        private CombatConfiguration currentCombat;
        public CombatConfiguration CurrentCombat
        {
            get
            {
                return currentCombat;
            }
            set 
            { 
                if(currentCombat == value)
                    return;

                currentCombat = value; 
                HordeCount = currentCombat.hordeSize;
                hordeDominoOptions = currentCombat.hordeDominos;
            }
        }
        private int hordeCount;
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

            foreach(DominoSpace space in survivorDominoSpaces)
                space.OnDominoSet += PlaceDomino;

            InputManager.Instance.gameControls.UI.Navigate.performed += CheckNullNavigation;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            InputManager.Instance.gameControls.UI.Navigate.performed -= CheckNullNavigation;
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

        public void StartCombat(CombatConfiguration configuration)
        {
            CurrentCombat = configuration;
            CurrentCombatPhase = CombatPhase.SETUP;
        }

        public void DiscardDomino(Domino domino)
        {
            discard.Add(domino.DominoConfiguration);
            Destroy(domino.gameObject);
        }

        #region Domino Movement Sequence
        private void TriggerSelectSequence(Domino domino)
        {
            if (transform.parent.GetComponent<DominoSpace>() == null)
            {
                if (selectSequence != null) selectSequence.Kill();
                if (deselectSequence != null) deselectSequence.Kill(true);
                selectSequence = DOTween.Sequence();
                selectSequence.Append(domino.transform.DOLocalMove(new Vector3(0, .4f, -1), selectionTransitionTime));
                selectSequence.Append(domino.transform.DOScale(domino.defaultScale * 1.35f, selectionTransitionTime));
                selectSequence.Play();
            }
        }

        private void TriggerDeselectSequence(Domino domino)
        {
            if (transform.parent.GetComponent<DominoSpace>() == null)
            {
                if (deselectSequence != null) deselectSequence.Kill(true);
                if (selectSequence != null) selectSequence.Kill();
                domino.transform.localScale = domino.defaultScale;
                deselectSequence = DOTween.Sequence();
                deselectSequence.Append(domino.transform.DOLocalMove(Vector2.zero, selectionTransitionTime));
                deselectSequence.Play();
                StartCoroutine(domino.CooldownMouseHover(selectionCooldownTime));
            }
        }

        private void TriggerDeconfirmSequence(Domino domino)
        {
            if (transform.parent.GetComponent<DominoSpace>() == null)
            {
                if (deselectSequence != null) deselectSequence.Kill(true);
                if (selectSequence != null) selectSequence.Kill();
                confirmedDomino.transform.localScale = confirmedDomino.defaultScale;
                deselectSequence = DOTween.Sequence();
                deselectSequence.Append(domino.transform.DOLocalMove(Vector2.zero, selectionTransitionTime));
                deselectSequence.Play();
            }
        }
        #endregion

        #region Game Board
        /// <summary>
        /// Updates chains associated with the given chainDomino, looking for new and expanded chains created by it
        /// </summary>
        /// <param name="chainDomino"></param>
        /// <param name="dominoSpaces"></param>
        private void UpdateDominoChains(Domino chainDomino, List<DominoSpace> dominoSpaces)
        {
            Dictionary<DominoChainType, List<Domino>> dominoChains= new Dictionary<DominoChainType, List<Domino>>();

            for(int i = 0; i < dominoSpaces.Count - 1; i++)
            {
                Domino thisDomino = dominoSpaces[i].Domino;
                Domino nextDomino = dominoSpaces[i + 1].Domino;
                
                if(thisDomino == null || nextDomino == null)
                    continue;

                // Make sure chain is not already accounted for, and then check if the two dominos have the same value
                if (!dominoChains.TryGetValue(DominoChainType.DEFENSE, out _) && thisDomino.DominoConfiguration.defenseEndType == nextDomino.DominoConfiguration.defenseEndType)
                {
                    if (thisDomino.DefenseCurrentStrength == nextDomino.DefenseCurrentStrength)
                    {
                        List<Domino> chain = new List<Domino>() { thisDomino, nextDomino };

                        for (int j = i + 2; j < dominoSpaces.Count; j++)
                        {
                            Domino domino = dominoSpaces[j].Domino;
                            if (domino != null
                                && domino.DominoConfiguration.defenseEndType == thisDomino.DominoConfiguration.defenseEndType
                                && domino.DefenseCurrentStrength == thisDomino.DefenseCurrentStrength)
                                chain.Add(domino);
                            else break;
                        }

                        if (chain.Contains(chainDomino))
                            dominoChains.Add(DominoChainType.DEFENSE, chain.Select(x => x).ToList());
                    }
                }

                // Repeat for the OFFENSE side
                if (!dominoChains.TryGetValue(DominoChainType.OFFENSE, out _) && thisDomino.DominoConfiguration.offenseEndType == nextDomino.DominoConfiguration.offenseEndType)
                {
                    if(thisDomino.OffenseCurrentStrength == nextDomino.OffenseCurrentStrength)
                    {
                        List<Domino> chain = new List<Domino>() { thisDomino, nextDomino };

                        for (int j = i + 2; j < dominoSpaces.Count; j++)
                        {
                            Domino domino = dominoSpaces[j].Domino;
                            if (domino != null
                                && domino.DominoConfiguration.offenseEndType == thisDomino.DominoConfiguration.offenseEndType
                                && domino.OffenseCurrentStrength == thisDomino.OffenseCurrentStrength)
                                chain.Add(domino);
                            else break;
                        }

                        if (chain.Contains(chainDomino))
                            dominoChains.Add(DominoChainType.OFFENSE, chain.Select(x => x).ToList());
                    }
                }
            }

            foreach(KeyValuePair<DominoChainType, List<Domino>> dominoChain in dominoChains)
            {
                if (!dominoChain.Value.Contains(chainDomino))
                    continue;

                if (dominoChain.Key == DominoChainType.DEFENSE)
                {
                    foreach (Domino domino in dominoChain.Value)
                        domino.DefenseCurrentStrength = dominoChain.Value.Select(x => x.DominoConfiguration.defenseEndStrength).Min() + dominoChain.Value.Count - 1;
                }

                if (dominoChain.Key == DominoChainType.OFFENSE)
                {
                    foreach (Domino domino in dominoChain.Value)
                            domino.OffenseCurrentStrength = dominoChain.Value.Select(x => x.DominoConfiguration.offenseEndStrength).Min() + dominoChain.Value.Count - 1;
                }
            }
        }
        #endregion
    }
}