using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.Zombinos
{
    public enum DestinationType
    {
        NONE,
        COMBAT
    }

    [System.Serializable]
    public class MapDestinationConfiguration
    {
        public Vector2 mapPosition;
        public DestinationType destinationType = DestinationType.NONE;
        public CombatConfiguration combatConfiguration;
    }
}