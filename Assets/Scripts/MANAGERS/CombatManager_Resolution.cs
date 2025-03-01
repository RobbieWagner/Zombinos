using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace RobbieWagnerGames.Zombinos
{
    public partial class CombatManager : MonoBehaviourSingleton<CombatManager>
    {
        public Action<CombatConfiguration, bool> OnEndCombat = (CombatConfiguration combatConfiguration, bool win) => { };

        private IEnumerator ResolveCombat(bool won = true)
        {
            if(won)
                Debug.Log("You win!!");
            else
                Debug.Log("You lose!!");

            yield return new WaitForSeconds(1.5f);

            OnEndCombat?.Invoke(CurrentCombat, won);

            GameManager.Instance.CurrentGameMode = GameMode.MENU;
        }
    }
}