using RobbieWagnerGames.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.Zombinos
{
    public class Map: MonoBehaviourSingleton<Map>
    {
        public Canvas canvas;
        public RectTransform mapView;
        public MapButton mapButtonPrefab;
        private List<MapButton> mapButtons = new List<MapButton>();

        public Color mapButtonLockedColor;
        public Color mapButtonCompleteColor;

        public void BuildMap(List<MapDestinationConfiguration> gameMapConfiguration)
        {
            canvas.enabled = true;

            foreach(MapButton button in mapButtons)
                Destroy(button.gameObject);
            mapButtons.Clear();

            foreach(MapDestinationConfiguration destination in gameMapConfiguration)
            {
                if (destination.destinationStatus == DestinationStatus.ACTIVE || destination.destinationStatus == DestinationStatus.LOCKED)
                    destination.destinationStatus = destination.prerequisites.Count > 0 ? DestinationStatus.LOCKED : DestinationStatus.ACTIVE;
                MapButton button = Instantiate(mapButtonPrefab, mapView);
                button.DestinationConfiguration = destination;
                Image buttonImage = button.GetComponent<Image>();
                buttonImage.sprite = GetMapButtonSprite(destination.destinationType);
                buttonImage.color = GetMapButtonColor(destination.destinationStatus);
                button.enabled = IsMapButtonEnabled(destination.destinationStatus);
                mapButtons.Add(button);
            }
        }

        private bool IsMapButtonEnabled(DestinationStatus destinationStatus)
        {
            if (destinationStatus == DestinationStatus.ACTIVE)
                return true;
            return false;
        }

        private Sprite GetMapButtonSprite(DestinationType destinationType)
        {
            switch (destinationType)
            {
                case DestinationType.BOSS:
                    return Resources.Load<Sprite>($"{StaticGameStats.mapButtonFilePath}BossBattleButton");
                case DestinationType.COMBAT:
                default:
                    return Resources.Load<Sprite>($"{StaticGameStats.mapButtonFilePath}Button1");
            }
        }

        private Color GetMapButtonColor(DestinationStatus destinationStatus)
        {
            switch (destinationStatus) 
            {
                case DestinationStatus.LOCKED:
                    return mapButtonLockedColor;
                case DestinationStatus.COMPLETED:
                    return mapButtonCompleteColor;
                case DestinationStatus.ACTIVE:
                default:
                    return Color.white;
            }
        }
    }
}