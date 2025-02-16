using RobbieWagnerGames.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

namespace RobbieWagnerGames.Zombinos
{
    public class DominoManager : MonoBehaviourSingleton<DominoManager>
    {
        public static Sprite GetDominoPipSprite(int pips)
        {
            return Resources.Load<Sprite>($"Sprites/Domino/pips_{pips}");
        }

        public static Sprite GetDominoAttributeSprite(DominoAttributeType attribute)
        {
            return Resources.Load<Sprite>($"Sprites/Domino/{attribute}");
        }
    }
}