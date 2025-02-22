using RobbieWagnerGames.Managers;
using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace RobbieWagnerGames.Zombinos
{
    public partial class CombatManager : MonoBehaviourSingleton<CombatManager>
    {
        private IEnumerator HandleExecutionPhase()
        {
            InputManager.Instance.gameControls.UI.RightClick.performed -= CancelSelection;
            InputManager.Instance.gameControls.UI.Cancel.performed -= CancelSelection;

            for (int i = 0; i < survivorDominoSpaces.Count; i++)
            {
                Domino survivorDomino = survivorDominoSpaces[i].Domino;
                Domino zombieDomino = zombieDominoSpaces[i].Domino;

                if (survivorDomino == null)
                    continue;

                for (int j = survivorDomino.OffenseCurrentStrength; j > 0; j--)
                {
                    if (zombieDomino == null)
                    {
                        HordeCount -= j;
                        break;
                    }
                    else
                    {
                        zombieDomino.DefenseCurrentStrength--;
                        if (zombieDomino.DefenseCurrentStrength == 0)
                        {
                            yield return new WaitForSeconds(.1f);
                            zombieDominoSpaces[i].Domino = null;
                        }
                    }

                    yield return new WaitForSeconds(.15f);
                }
            }

            for (int i = 0; i < zombieDominoSpaces.Count; i++)
            {
                Domino survivorDomino = survivorDominoSpaces[i].Domino;
                Domino zombieDomino = zombieDominoSpaces[i].Domino;

                if (zombieDomino == null)
                    continue;

                for (int j = zombieDomino.OffenseCurrentStrength; j > 0; j--)
                {
                    if (survivorDomino == null)
                    {
                        currentSurvivors[i].HP -= j;
                        break;
                    }
                    else
                    {
                        survivorDomino.DefenseCurrentStrength--;
                        if (survivorDomino.DefenseCurrentStrength == 0)
                        {
                            yield return new WaitForSeconds(.1f);
                            survivorDominoSpaces[i].Domino = null;
                        }
                    }

                    yield return new WaitForSeconds(.15f);
                }
            }

            CurrentCombatPhase = CombatPhase.TURN_END;
        }
    }
}