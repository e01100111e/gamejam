using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CatFight
{
    [Serializable]
    public class AdventureFight
    {
        public Wave wave;
        public bool isNextFight;
    }

    [Serializable]
    public class Adventure
    {

        public List<AdventureFight> waves = new List<AdventureFight>();
        public GameObject dialoguePrefabs;
        public int adventureNumber;
        public CharacterData catToRescue;
        public string adventureName;
        public string adventureDetails;
        public List<string> adventureStoryMoments;

        public Adventure(AdventureData adventureData)
        {
            adventureNumber = adventureData.AdventureNumber;
            catToRescue = adventureData.CatToRescue;
            adventureName = adventureData.AdventureName;
            adventureDetails = adventureData.AdventureDetails;
            foreach (var fight in adventureData.Fights)
            {
                Wave wave = new(fight.waveData);
                AdventureFight _fight = new();
                _fight.wave = wave;
                _fight.isNextFight = fight.isNextFight;
                waves.Add(_fight);
            }
            adventureStoryMoments = new List<string>(adventureData.AdventureStoryMoments);
        }

    }
}
