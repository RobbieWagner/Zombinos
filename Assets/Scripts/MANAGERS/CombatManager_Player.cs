using DG.Tweening;
using RobbieWagnerGames.Managers;
using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.Zombinos
{
    public partial class CombatManager : MonoBehaviourSingleton<CombatManager>
    {
        private bool isTurnComplete = false;
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
                
                if (selectedDomino != null)
                {
                    selectedDomino.transform.DOLocalMove(Vector3.zero, .4f);
                    selectedDomino.transform.DOScale(selectedDomino.defaultScale, .4f);
                }

                selectedDomino = value;

                if (selectedDomino != null)
                {
                    selectedDomino.transform.DOLocalMove(new Vector3(0, 1, -1), .4f);
                    selectedDomino.transform.DOScale(selectedDomino.defaultScale * 1.5f, .4f);
                }

                OnSetSelectedDomino?.Invoke(value);
            } 
        }
        public Action<Domino> OnSetSelectedDomino = (Domino domino) => { };
        public Domino ConfirmedDomino { get; set; }

        private IEnumerator HandlePlayerPhase()
        {
            isTurnComplete = false;

            StartHandSelection();

            InputManager.Instance.gameControls.UI.Cancel.performed += CancelSelection;
            InputManager.Instance.EnableActionMap(ActionMapName.UI.ToString());

            while (!isTurnComplete)
                yield return null;

            currentCombatPhase = CombatPhase.EXECUTION;
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
                playerHand.Remove(domino);
                foreach(Transform t in handTransforms.Where(x => x.childCount == 0))
                    Destroy(t);
            }

            if (survivorDominoSpaces.Where(x => x.Domino == null).Any())
                StartHandSelection();
            else
                isTurnComplete = true;
        }

        private void StartHandSelection()
        {
            EventSystemManager.Instance.SetSelectedGameObject(playerHand[0].button.gameObject);
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