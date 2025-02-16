using RobbieWagnerGames.Managers;
using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.Zombinos
{
    public partial class CombatManager : MonoBehaviourSingleton<CombatManager>
    {
        private IEnumerator StartTurn()
        {
            foreach (DominoSpace space in zombieDominoSpaces)
            {
                if (space.Domino == null && HordeCount > 0)
                {
                    yield return StartCoroutine(SpawnZombieDomino(space));
                }
                yield return new WaitForSeconds(.2f);
            }

            while (playerHand.Count < playerHandSize)
            {
                yield return StartCoroutine(DrawDomino());
            }

            CurrentCombatPhase = CombatPhase.PLAYER;
        }

        private IEnumerator SpawnZombieDomino(DominoSpace space, bool isFromHorde = true)
        {
            // TODO: Spawn in a "pretty" way using tweens
            yield return null;

            DominoConfiguration config = hordeDominoOptions[UnityEngine.Random.Range(0, hordeDominoOptions.Count)];

            Domino newDomino = Instantiate(enemyDominoPrefab, space.transform);
            newDomino.DominoConfiguration = config;
            
            space.Domino = newDomino;

            if (isFromHorde)
                HordeCount--;
        }

        private IEnumerator DrawDomino()
        {
            if(currentDeck.Count == 0)
            {
                if (discard.Count == 0)
                    currentDeck = baseDeck;
                else
                    currentDeck = discard;

                ShuffleDominos();
            }

            DominoConfiguration dominoConfig = currentDeck[0];
            yield return StartCoroutine(AddDominoToHand(dominoConfig));
            currentDeck.RemoveAt(0);
        }

        private IEnumerator AddDominoToHand(DominoConfiguration configuration)
        {
            yield return new WaitForSeconds(.2f);
            handTransforms.Add(Instantiate(handTransformPrefab, playerHandParent));
            Domino newDomino = Instantiate(playerDominoPrefab, handTransforms.Last());
            newDomino.DominoConfiguration = configuration;
            playerHand.Add(newDomino);
            UpdateHandVisual();
        }

        private void UpdateHandVisual()
        {
            for(int i = 0; i < playerHand.Count; i++)
            {
                Domino domino = playerHand[i];

                Navigation navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = i == 0 ? null : playerHand[i - 1].button,
                    selectOnRight = i == playerHand.Count - 1 ? null : playerHand[i + 1].button,
                };
                domino.button.navigation = navigation;

                domino.transform.localPosition = Vector3.zero;
                handTransforms[i].position = new Vector3 (handTransforms[i].position.x, handTransforms[i].position.y, - 1 - (i * .1f));
            }
        }

        private void ShuffleDominos()
        {
            for (int i = currentDeck.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i+1);
                (currentDeck[i], currentDeck[j]) = (currentDeck[j], currentDeck[i]);
            }
        }

        private IEnumerator EndTurn()
        {
            yield return null;
        }
    }
}