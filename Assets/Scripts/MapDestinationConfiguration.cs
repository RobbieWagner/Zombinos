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

    public class MapDestinationConfiguration : MonoBehaviour
    {
        public Vector2 mapPosition;
        public string gameScene;
        public DestinationType destinationType = DestinationType.NONE;
    }
}