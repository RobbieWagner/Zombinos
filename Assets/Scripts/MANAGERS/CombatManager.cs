using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.Zombinos
{
    public enum CombatPhase
    {
        SETUP,
        TURN_START,
        PLAYER,
        EXECUTION,
        TURN_END,
        WIN,
        LOSE
    }

    public class CombatManager : MonoBehaviour
    {
        [SerializeField] private Domino dominoPrefab;
    }
}