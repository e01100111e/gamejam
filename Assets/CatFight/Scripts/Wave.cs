using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CatFight
{
    [Serializable]
    public class Wave
    {
        public int levelNumber = 0;
        public List<Character> enemies = new();
        public Sprite background;

        public bool isNextStepFight;

        public int treatAward;
        public Wave(WaveData data)
        {

            levelNumber = data.WaveNumber;
            foreach (var enemyData in data.Enemies)
            {
                Character enemy = new(enemyData);
                enemies.Add(enemy);
            }
            background = data.Background;
            treatAward = data.TreatAward;
        }
    }
}
