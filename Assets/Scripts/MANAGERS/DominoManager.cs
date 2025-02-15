using RobbieWagnerGames.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

namespace RobbieWagnerGames.Zombinos
{
    public class DominoManager : MonoBehaviourSingleton<DominoManager>
    {
        public Sprite GetDominoPipSprite(int pips)
        {
            return Resources.Load<Sprite>($"Sprites/Domino/pips_{pips}");
        }

        public Sprite GetDominoAbilitySprite(DominoAbility ability)
        {
            return Resources.Load<Sprite>($"Sprites/Domino/{ability}");
        }
    }
}