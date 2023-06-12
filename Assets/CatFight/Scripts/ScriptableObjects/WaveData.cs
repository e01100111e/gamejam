using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    //Add menu item to create a new WaveData asset
    [CreateAssetMenu(fileName = "New WaveData", menuName = "CatFight/WaveData")]
    public class WaveData : ScriptableObject
    {
        [SerializeField] private int m_Level = 0;
        [SerializeField] private List<CharacterData> enemies = new();
        [SerializeField] private Sprite background;
        [SerializeField] private string levelName;

        [SerializeField] private string levelDetails = "WaveData Details";
        [SerializeField] private int treatAward = 0;
        public int WaveNumber => m_Level;
        public List<CharacterData> Enemies => enemies;
        public Sprite Background => background;
        public string LevelDetails => levelDetails;
        public string LevelName => levelName;
        public int TreatAward => treatAward;



    }
}
