using System;
using UnityEngine;

namespace RobbieWagnerGames
{
    public static class StaticGameStats
    {
        #region Asset File Paths
        //All file paths are local to the "Resources" folder.
        public static string combatActionFilePath = "Unit";
        public static string spritesFilePath = "Sprites/";
        public static string characterSpriteFilePath = "Sprites/Characters/";
        public static string survivorSpritesFilePath = "Sprites/Characters/Survivors/";
        public static string backgroundSpriteFilePath = "Sprites/Backgrounds/";
        public static string headSpriteFilePath = "Sprites/Heads";

        public static string mapButtonFilePath = "Sprites/UI/Map/";

        public static string combatAnimatorFilePath = "Animation/CombatAnimation";
        public static string defaultCombatAnimatorFilePath = "Animation/CombatAnimation/Player/Player";
        
        public static string soundFilePath = "Sounds/";
        public static string dialogueMusicFilePath = "Sounds/Dialogue/Music/";
        public static string dialogueSoundEffectsFilePath = "Sounds/Dialogue/SoundEffects/";
        
        public static string combatMusicFilePath = "Sounds/Combat/Music/";
        public static string combatSoundEffectsFilePath = "Sounds/Combat/SoundEffects/";
        //TODO: find way to load scene in build!!
        public static string sceneFilePath = "Assets/Scenes/Combat/";

        public static string dialogueSavePath = "Exploration/DialogueInteractions/";

        #endregion

        #region Save Data
        private static string persistentDataPath;
        public static string PersistentDataPath
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(persistentDataPath))
                    persistentDataPath = Application.persistentDataPath; 
                return persistentDataPath; 
            }
            private set 
            { 
                persistentDataPath = value; 
            }
        }
        public static string partySavePath = "player_party.json";
        public static string inventorySavePath = "player_inventory.json";
        public static string explorationDataSavePath = "exploration_data.json";
        public static string gameDataSavePath = "game_data.json";
        public static string mapConfigurationSavePath = "map_configuration.json";
        #endregion
    }
}