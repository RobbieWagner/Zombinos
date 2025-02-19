using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace RobbieWagnerGames.Zombinos
{
    public partial class CombatManager : MonoBehaviourSingleton<CombatManager>
    {
        private IEnumerator ResolveCombat(bool won = true)
        {
            if(won)
                Debug.Log("You win!!");
            else
                Debug.Log("You lose!!");

            yield return null;
        }
    }
}