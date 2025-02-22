using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.Zombinos
{
    public class Map: MonoBehaviourSingleton<Map>
    {
        public Canvas canvas;
        public RectTransform mapView;
        public List<MapDestinationConfiguration> mapDestinations;
        public MapButton mapButtonPrefab;
        private List<MapButton> mapButtons = new List<MapButton>();

        private void Awake()
        {
            BuildMap();
        }

        private void BuildMap()
        {
            foreach(MapButton button in mapButtons)
                Destroy(button.gameObject);
            mapButtons.Clear();

            foreach(MapDestinationConfiguration destination in mapDestinations)
            {
                MapButton button = Instantiate(mapButtonPrefab, mapView);
                button.DestinationConfiguration = destination;
                mapButtons.Add(button);
            }
        }
    }
}