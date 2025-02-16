using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using UnityEngine.UI;

namespace RobbieWagnerGames.Zombinos
{
    public partial class CombatManager : MonoBehaviourSingleton<CombatManager>
    {
        private IEnumerator SetupCombat()
        {
            yield return null;
            
            for (int i = 0; i < survivorDominoSpaces.Count; i++) 
            {
                DominoSpace survivorSpace = survivorDominoSpaces[i];
                DominoSpace zombieSpace = zombieDominoSpaces[i];
                survivorSpace.interactable = false;

                Navigation survivorNavigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = i == 0 ? null : survivorDominoSpaces[i - 1],
                    selectOnRight = i == survivorDominoSpaces.Count - 1 ? null : survivorDominoSpaces[i + 1],
                    selectOnUp = zombieSpace
                };
                survivorSpace.navigation = survivorNavigation;

                Navigation zombieNavigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = i == 0 ? null : zombieDominoSpaces[i - 1],
                    selectOnRight = i == zombieDominoSpaces.Count - 1 ? null : zombieDominoSpaces[i + 1],
                    selectOnDown = survivorSpace
                };
                survivorSpace.navigation = zombieNavigation;
            }

            currentSurvivors.Clear();
            for(int i = 0; i < 3; i++)
            {
                currentSurvivors.Add(GameManager.Instance.playerParty[i]);
                survivorUis[i].Survivor = currentSurvivors[i];
                currentSurvivors[i].HP = currentSurvivors[i].survivorInfo.maxHP;
            }

            CurrentCombatPhase = CombatPhase.TURN_START;
        }
    }
}