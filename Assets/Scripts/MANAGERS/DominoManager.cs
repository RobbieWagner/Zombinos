using DG.Tweening;
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

        public IEnumerator UpdateSpriteCo(SpriteRenderer spriteRenderer, int newValue)
        {
            Color spriteColor = spriteRenderer.color;
            yield return spriteRenderer.DOColor(Color.clear, .25f).SetEase(Ease.InCubic).WaitForCompletion();
            spriteRenderer.sprite = GetDominoPipSprite(newValue);
            yield return spriteRenderer.DOColor(spriteColor, .25f).SetEase(Ease.OutCubic).WaitForCompletion();
        }
    }
}