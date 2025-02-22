using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace RobbieWagnerGames.Zombinos
{
    public class MapButton : MonoBehaviour
    {
        [SerializeField] private ButtonListener button;
        [SerializeField] private RectTransform rectTransform;

        private MapDestinationConfiguration destinationConfiguration = null;
        public MapDestinationConfiguration DestinationConfiguration
        {
            get 
            {
                return destinationConfiguration;
            }
            set 
            {
                if(destinationConfiguration == value)
                    return;

                destinationConfiguration = value;
                rectTransform.anchoredPosition = destinationConfiguration.mapPosition;
            }
        }

        private void Awake()
        {
            button.onClick.AddListener(OnMapButtonClicked);
        }

        private void OnMapButtonClicked()
        {
            Map.Instance.canvas.enabled = false;
            SceneManager.LoadScene(destinationConfiguration.gameScene, LoadSceneMode.Additive);
            //TODO: Add functionality to set game mode here
        }
    }
}