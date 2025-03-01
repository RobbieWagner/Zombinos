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

                if (zombieDomino == null)
                {
                    if (hordeCount == 0) continue;
                    HordeCount -= survivorDomino.OffenseCurrentStrength;
                }
                else
                {
                    int power = survivorDomino.OffenseCurrentStrength;

                    if (power >= zombieDomino.DefenseCurrentStrength)
                    {
                        power -= zombieDomino.DefenseCurrentStrength;
                        zombieDomino.DefenseCurrentStrength = 0;
                        zombieDominoSpaces[i].Domino = null;
                    }
                    else
                    {
                        zombieDomino.DefenseCurrentStrength -= power;
                        power = 0;
                    }

                    HordeCount -= power;
                }

                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(.5f);

            for (int i = 0; i < zombieDominoSpaces.Count; i++)
            {
                Domino survivorDomino = survivorDominoSpaces[i].Domino;
                Domino zombieDomino = zombieDominoSpaces[i].Domino;

                if (zombieDomino == null)
                    continue;

                if (survivorDomino == null)
                {
                    if (currentSurvivors[i].HP == 0) continue;
                    currentSurvivors[i].HP -= zombieDomino.OffenseCurrentStrength;
                }
                else
                {
                    int power = zombieDomino.OffenseCurrentStrength;

                    if (power >= survivorDomino.DefenseCurrentStrength)
                    {
                        power -= survivorDomino.DefenseCurrentStrength;
                        survivorDomino.DefenseCurrentStrength = 0;
                        survivorDominoSpaces[i].Domino = null;
                    }
                    else
                    {
                        survivorDomino.DefenseCurrentStrength -= power;
                        power = 0;
                    }

                    currentSurvivors[i].HP -= power;
                }

                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(.5f);
            CurrentCombatPhase = CombatPhase.TURN_END;
        }
    }
}