using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.Zombinos
{
    public enum DominoAttributeType
    {
        NONE
    }

    [Serializable]
    public class DominoConfiguration
    {
        public DominoAttributeType offenseEndType;
        public int offenseEndStrength;
        public DominoAttributeType defenseEndType;
        public int defenseEndStrength;
    }
}