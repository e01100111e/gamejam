using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    [Serializable]
    public class AdventureFightData
    {
        public WaveData waveData;
        public bool isNextFight;
    }

    //Add menu item to create a new WaveData asset
    [CreateAssetMenu(fileName = "New AdventureData", menuName = "CatFight/AdventureData")]
    public class AdventureData : ScriptableObject
    {
        [SerializeField] private int number = 0;
        [SerializeField] private List<AdventureFightData> fights = new();
        [SerializeField] private string adventureName;
        [SerializeField] private string adventureDetails;

        [SerializeField] private CharacterData catToRescue;

        [SerializeField] private List<string> adventureStoryMoments = new();

        [SerializeField] private string levelDetails = "WaveData Details";

        [SerializeField] private bool startingAdventure;
        public int AdventureNumber => number;
        public List<AdventureFightData> Fights => fights;
        public CharacterData CatToRescue => catToRescue;
        public string LevelDetails => levelDetails;
        public string AdventureName => adventureName;
        public string AdventureDetails => adventureDetails;
        public bool StartingAdventure => startingAdventure;

        public List<string> AdventureStoryMoments => adventureStoryMoments;
       



    }
}
