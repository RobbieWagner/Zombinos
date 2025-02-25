using RobbieWagnerGames.Managers;
using RobbieWagnerGames.Utilities;
using RobbieWagnerGames.Utilities.SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RobbieWagnerGames.Zombinos
{
    public enum GameMode
    {
        NONE,
        MENU,
        COMBAT
    }

    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        #region player party
        public List<Survivor> playerParty;
        private static List<SurvivorInfo> defaultParty = new List<SurvivorInfo>() 
        { 
            new SurvivorInfo() 
            { 
                maxHP = 20, 
                survivorSpritePath = "Survivor1"
            },
            new SurvivorInfo()
            {
                maxHP = 20, survivorSpritePath = "Survivor2"
            },
            new SurvivorInfo()
            {
                maxHP = 20, survivorSpritePath = "Survivor3"
            }
        };
        #endregion

        public static Action<GameMode> OnGameModeChanged = (GameMode gameMode) => { };
        public static Action<GameMode, GameMode> OnGameModeEnded = (GameMode gameMode, GameMode nextGameMode) => { };
        private GameMode previousGameMode = GameMode.NONE;
        private GameMode currentGameMode = GameMode.NONE; //GameMode.NONE //TODO: HAVE GAMEMODE INITIALIZED TO NONE
        public GameMode CurrentGameMode
        {
            get
            {
                return currentGameMode;
            }
            set
            {
                if (value == currentGameMode)
                    return;
                OnGameModeEnded?.Invoke(currentGameMode, value);
                previousGameMode = currentGameMode;
                currentGameMode = value;
                OnGameModeChanged?.Invoke(currentGameMode);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (JsonDataService.Instance == null)
                new JsonDataService();

            OnGameModeEnded += OnEndGameMode;
            OnGameModeChanged += OnChangeGameMode;

            LoadGameData();
            StartCoroutine(StartGame());
        }

        private IEnumerator StartGame()
        {
            yield return null;
            CurrentGameMode = GameMode.MENU;
        }

        private void OnEndGameMode(GameMode mode, GameMode nextGameMode)
        {
            if (mode == GameMode.MENU)
            {
                Map.Instance.canvas.enabled = false;
                EventSystemManager.Instance.SetSelectedGameObject(null);
            }
            else if (mode == GameMode.COMBAT)
            {
                InputManager.Instance.gameControls.COMBAT.Disable();
                EventSystemManager.Instance.SetSelectedGameObject(null);
                UnloadCombatScene();
            }
        }

        private void OnChangeGameMode(GameMode mode)
        {
            if (mode == GameMode.MENU)
            {
                OpenMap();
            }
        }

        public void LoadGameData()
        {
            List<SurvivorInfo> savedPartyInfo = JsonDataService.Instance.LoadDataRelative(StaticGameStats.partySavePath, defaultParty);
            if (savedPartyInfo != null)
            {
                playerParty = new List<Survivor>();
                foreach(SurvivorInfo survivorInfo in savedPartyInfo)
                {
                    Survivor survivor = new Survivor();
                    survivor.survivorInfo = survivorInfo;
                    playerParty.Add(survivor);
                }
            }
        }

        public void SaveGameData()
        {
            JsonDataService.Instance.SaveData(StaticGameStats.partySavePath, playerParty.Select(x => x.survivorInfo));
        }

        public void OpenMap()
        {
            Map.Instance.BuildMap();
        }

        public void TriggerCombat(CombatConfiguration combatConfiguration)
        {
            CurrentGameMode = GameMode.COMBAT;
            StartCoroutine(TriggerCombatCo(combatConfiguration));
        }

        private IEnumerator TriggerCombatCo(CombatConfiguration combatConfiguration)
        {
            SceneManager.LoadScene(combatConfiguration.combatSceneName, LoadSceneMode.Additive);
            while (CombatManager.Instance == null)
                yield return null;

            CombatManager.Instance.StartCombat(combatConfiguration);
        }

        private void UnloadCombatScene()
        {
            SceneManager.UnloadSceneAsync(CombatManager.Instance.gameObject.scene);
        }
    }
}