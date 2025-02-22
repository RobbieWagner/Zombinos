using DG.Tweening;
using RobbieWagnerGames.Managers;
using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.Zombinos
{
    public partial class CombatManager : MonoBehaviourSingleton<CombatManager>
    {
        private Sequence selectSequence;
        private Sequence deselectSequence;
        public float selectionTransitionTime = .4f;
        public float selectionCooldownTime = .2f;
        private bool isPlayerFinished = false;
        private Domino selectedDomino;
        public Domino SelectedDomino 
        {
            get 
            { 
                return selectedDomino; 
            } 
            set
            {
                if (value == selectedDomino)
                    return;
                
                if (selectedDomino != null && selectedDomino != ConfirmedDomino)
                {
                    if (deselectSequence != null) deselectSequence.Kill(true);
                    if (selectSequence != null) selectSequence.Kill();
                    selectedDomino.transform.localScale = selectedDomino.defaultScale;
                    deselectSequence = DOTween.Sequence();
                    deselectSequence.Append(selectedDomino.transform.DOLocalMove(Vector3.zero, selectionTransitionTime));
                    deselectSequence.Play();
                    StartCoroutine(selectedDomino.CooldownMouseHover(selectionCooldownTime));
                }

                selectedDomino = value;

                if (selectedDomino != null)
                {
                    if (selectSequence != null) selectSequence.Kill();
                    if (deselectSequence != null) deselectSequence.Kill(true);
                    selectSequence = DOTween.Sequence();
                    selectSequence.Append(selectedDomino.transform.DOLocalMove(new Vector3(0, .4f, -1), selectionTransitionTime));
                    selectSequence.Append(selectedDomino.transform.DOScale(selectedDomino.defaultScale * 1.35f, selectionTransitionTime));
                    selectSequence.Play();
                }

                OnSetSelectedDomino?.Invoke(value);
            } 
        }
        public Action<Domino> OnSetSelectedDomino = (Domino domino) => { };
        
        private Domino confirmedDomino = null;
        public Domino ConfirmedDomino
        {
            get
            {
                return confirmedDomino;
            }
            set
            {
                if (confirmedDomino == value)
                    return;

                if (confirmedDomino != null)
                {
                    if (deselectSequence != null) deselectSequence.Kill(true);
                    if (selectSequence != null) selectSequence.Kill();
                    confirmedDomino.transform.localScale = confirmedDomino.defaultScale;
                    deselectSequence = DOTween.Sequence();
                    deselectSequence.Append(confirmedDomino.transform.DOLocalMove(Vector3.zero, selectionTransitionTime));
                    deselectSequence.Play();
                }
                
                confirmedDomino = value;
                OnSetConfirmedDomino?.Invoke(value);

                if (value != null)
                    StartDominoPlacement();
                else
                {
                    foreach (DominoSpace space in survivorDominoSpaces)
                        space.button.interactable = false;
                }
            }
        }
        public Action<Domino> OnSetConfirmedDomino = (Domino domino) => { };

        private IEnumerator HandlePlayerPhase()
        {
            isPlayerFinished = false;

            StartHandSelection();

            InputManager.Instance.gameControls.UI.RightClick.performed += CancelSelection;
            InputManager.Instance.gameControls.UI.Cancel.performed += CancelSelection;
            InputManager.Instance.EnableActionMap(ActionMapName.UI.ToString());

            while (!isPlayerFinished)
                yield return null;

            foreach (DominoSpace space in survivorDominoSpaces)
                space.button.interactable = false;

            CurrentCombatPhase = CombatPhase.EXECUTION;
        }

        private void CancelSelection(InputAction.CallbackContext context)
        {
            if (ConfirmedDomino != null)
            {
                ConfirmedDomino = null;
                StartHandSelection();
            }    
        }

        private void PlaceDomino(Domino domino, DominoSpace space)
        {
            if (playerHand.Contains(domino))
            {
                if (selectSequence != null) selectSequence.Kill();

                playerHand.Remove(domino);
                foreach (Transform t in handTransforms.Where(x => x.childCount == 0))
                    Destroy(t.gameObject);
                handTransforms = handTransforms.Where(x => x.childCount > 0).ToList();
                domino.transform.localScale = domino.defaultScale;
                if (survivorDominoSpaces.Where(x => x.Domino == null).Any())
                    StartHandSelection();
                else
                    isPlayerFinished = true;
            }
        }

        private void StartHandSelection()
        {
            ConfirmedDomino = null;

            foreach (Domino handDomino in playerHand)
                handDomino.button.interactable = true;

            EventSystemManager.Instance.SetSelectedGameObject(playerHand.First().button.gameObject);
        }
        
        private void StartDominoPlacement()
        {
            foreach (DominoSpace space in survivorDominoSpaces.Where(x => x.Domino == null))
                space.button.interactable = true;
            foreach (Domino domino in playerHand)
                domino.button.interactable = false;

            EventSystemManager.Instance.SetSelectedGameObject(survivorDominoSpaces.Where(x => x.Domino == null).First().button.gameObject);
        }

        private void CheckNullNavigation(InputAction.CallbackContext context)
        {
            // TODO: Test, make smarter (this is to correct any issues with ui navigation when switching between controls)
            if(CurrentCombatPhase == CombatPhase.PLAYER && EventSystemManager.Instance.eventSystem.currentSelectedGameObject == null)
            {
                if (ConfirmedDomino != null)
                    EventSystemManager.Instance.SetSelectedGameObject(survivorDominoSpaces.First().button.gameObject);
                else
                    EventSystemManager.Instance.SetSelectedGameObject(playerHand.First().button.gameObject);
            }
        }
    }
}