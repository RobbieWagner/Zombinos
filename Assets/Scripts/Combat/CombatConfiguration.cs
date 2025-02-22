using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.Zombinos
{
    [System.Serializable]
    public class CombatConfiguration
    {
        public int hordeSize;
        public List<DominoConfiguration> hordeDominos;
        public string combatSceneName;
    }
}