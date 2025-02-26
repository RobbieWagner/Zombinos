using RobbieWagnerGames.Utilities;
using System;
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

        public IEnumerator UpdateSpriteCo(SpriteRenderer spriteRenderer, int oldValue, int newValue)
        {
            int min = Math.Min(oldValue, newValue);
            int max = Math.Max(oldValue, newValue);

            for (int i = min + 1; i <= max; i++)
            {
                spriteRenderer.sprite = GetDominoPipSprite(i);
                yield return new WaitForSeconds(.15f);
            }
        }
    }
}