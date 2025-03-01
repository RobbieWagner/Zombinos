using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.Zombinos
{
    public enum DestinationType
    {
        NONE,
        COMBAT,
        BOSS
    }

    public enum DestinationStatus
    {
        NONE,
        ACTIVE,
        LOCKED,
        COMPLETED
    }

    [System.Serializable]
    public class MapDestinationConfiguration
    {
        public float mapPositionX;
        public float mapPositionY;
        public DestinationType destinationType = DestinationType.NONE;
        public DestinationStatus destinationStatus = DestinationStatus.NONE;
        public CombatConfiguration combatConfiguration;
        public List<string> prerequisites = new List<string>();
    }
}