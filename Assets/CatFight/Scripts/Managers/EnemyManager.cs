using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace CatFight
{
    public class EnemyManager : MonoBehaviour
    {
        List<Card> enemyHand = new();
        Queue<CharacterUI> randomOrderQueue = new();

        public static EnemyManager Instance;

        public bool enemyPlaying = false;
        private void Awake()
        {
            Instance = this;
        }

        public IEnumerator EnemyPlayingItsTurn()
        {
            while (randomOrderQueue.Count > 0)
            {
                enemyPlaying = true;
                randomOrderQueue.Dequeue().SelectThisCharacter();
                List<CharacterUI> targets = UIManager.Instance.Allies.FindAll(x => x.gameObject.activeInHierarchy && x.CurrentCharacter.IsTargetable(isAttacked:true));

                if (targets.Count > 0)
                {
                    BattleManager.Instance.TargetedCharacter = targets[Random.Range(0, targets.Count)];
                    if (BattleManager.Instance.TargetedCharacter != null)
                    {
                        Card cardToPlay = enemyHand[Random.Range(0, enemyHand.Count)];
                        cardToPlay.cardActionCompleted = false;
                        cardToPlay.PlayCard();
                        yield return new WaitForSeconds(1f);
                    }
                }
                

                // Wait for a short time before processing the next object
                yield return new WaitForSeconds(1f);
            }

            enemyPlaying = false;
        }
        //Create random order queue from enemies
        public void CreateRandomOrderQueue()
        {
            List<CharacterUI> enemies = UIManager.Instance.Enemies.FindAll(x => x.CurrentCharacter!= null && x.CurrentCharacter.CanAttack());
            randomOrderQueue = new();
            while (enemies.Count > 0)
            {
                int randomIndex = Random.Range(0, enemies.Count);
                randomOrderQueue.Enqueue(enemies[randomIndex]);
                enemies.RemoveAt(randomIndex);
            }
        }
        //public void PickRandomTarget()
        //{
        //    List<CharacterUI> aliveTargets = UIManager.Instance.Allies.FindAll(x => x.gameObject.activeInHierarchy && !x.CurrentCharacter.IsDead());

        //    if (aliveTargets.Count > 0)
        //    {
        //        BattleManager.Instance.TargetedCharacter = aliveTargets[Random.Range(0, aliveTargets.Count)];
        //    }
        //}

        ////Play a random card from currentPlayingEnemy's hand
        //public void PlayRandomCard()
        //{
        //    if (BattleManager.Instance.TargetedCharacter != null)
        //    {
        //        Card cardToPlay = enemyHand[Random.Range(0, enemyHand.Count)];
        //        PickRandomTarget();

        //        cardToPlay.PlayCard();
        //    }
        //}

        public void LoadEnemyHand()
        {
            foreach (var enemy in BattleManager.Instance.Enemies)
            {
                List<Card> cardDatas = enemy.cardList;

                foreach (var card in cardDatas)
                {
                    enemyHand.Add(card);
                }
            }
        }


    }
}
