﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Grids2D;
using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Characters.Allies;
using NueGames.NueDeck.Scripts.Characters.Enemies;
using NueGames.NueDeck.Scripts.Data.Containers;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Utils.Background;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Managers
{
    public class CombatManager : MonoBehaviour
    {
        private CombatManager(){}
        public static CombatManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private BackgroundContainer backgroundContainer;
        [SerializeField] private List<Transform> enemyPosList;
        [SerializeField] private List<Transform> allyPosList;
        [SerializeField] private Grid2D tacticalGrid;
        [SerializeField] private Alien alienPrefab;
        [SerializeField] private Soldier soldierPrefab;
        [SerializeField] private PowerStation powerStationPrefab;
        //[SerializeField] private GridSquare gridVisual;
        //[SerializeField] private GridMaster tacticalGrid;
        //[SerializeField] private PathFindingManager pathFinder;

        private bool alienSpawnQueued = false;
        private Vector3 alienSpawnPos = Vector3.zero;
        private int alientTargetIndex = -1;

        #region Cache
        public List<EnemyBase> CurrentEnemiesList { get; private set; } = new List<EnemyBase>();
        public List<AllyBase> CurrentAlliesList { get; private set; }= new List<AllyBase>();

        public Action OnAllyTurnStarted;
        public Action OnEnemyTurnStarted;
        public List<Transform> EnemyPosList => enemyPosList;

        public List<Transform> AllyPosList => allyPosList;

        public AllyBase CurrentMainAlly => CurrentAlliesList.Count>0 ? CurrentAlliesList[0] : null;

        public EnemyEncounter CurrentEncounter { get; private set; }

        public Grid2D TacticalGrid => tacticalGrid;

        public CombatStateType CurrentCombatStateType
        {
            get => _currentCombatStateType;
            private set
            {
                ExecuteCombatState(value);
                _currentCombatStateType = value;
            }
        }

        private CombatStateType _currentCombatStateType;
        protected FxManager FxManager => FxManager.Instance;
        protected AudioManager AudioManager => AudioManager.Instance;
        protected GameManager GameManager => GameManager.Instance;
        protected UIManager UIManager => UIManager.Instance;

        protected CollectionManager CollectionManager => CollectionManager.Instance;

        #endregion


        #region Setup
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                Instance = this;
                CurrentCombatStateType = CombatStateType.PrepareCombat;
            }
        }

        private void Start()
        {
            //gridVisual.Initialize();
            StartCombat();
        }

        private void Update()
        {
            foreach (Cell cell in tacticalGrid.cells)
            {
                //if (cell.canCross)
                //{
                //    tacticalGrid.CellSetColor(cell.index, new Color(0.0f, 0.0f, 1.0f, 0.5f));
                //}
                //else
                //{
                //    tacticalGrid.CellSetColor(cell.index, new Color(1.0f, 0.0f, 0.0f, 0.5f));
                //}

                switch (cell.tag)
                {
                    case (int)CellTags.Alien:
                        tacticalGrid.CellSetColor(cell.index, new Color(0.0f, 1.0f, 0.0f, 0.25f));

                        break;

                    case (int)CellTags.AlienPod:
                        tacticalGrid.CellSetColor(cell.index, new Color(0.0f, 1.0f, 1.0f, 0.25f));

                        break;

                    case (int)CellTags.Population:
                        tacticalGrid.CellSetColor(cell.index, new Color(1.0f, 1.0f, 1.0f, 0.25f));

                        break;

                    case (int)CellTags.PowerStation:
                        tacticalGrid.CellSetColor(cell.index, new Color(1.0f, 1.0f, 0.0f, 0.25f));

                        break;

                    case (int)CellTags.Soldier:
                        tacticalGrid.CellSetColor(cell.index, new Color(0.0f, 0.0f, 1.0f, 0.25f));

                        break;

                    default:
                        tacticalGrid.CellSetColor(cell.index, new Color(0.0f, 0.0f, 0.0f, 0.0f));

                        break;
                }
            }
        }

        public void StartCombat()
        {
            BuildEnemies();
            BuildAllies();
            backgroundContainer.OpenSelectedBackground();

            CollectionManager.SetGameDeck();

            UIManager.CombatCanvas.gameObject.SetActive(true);
            UIManager.InformationCanvas.gameObject.SetActive(true);
            CurrentCombatStateType = CombatStateType.AllyTurn;
        }

        private void ExecuteCombatState(CombatStateType targetStateType)
        {
            switch (targetStateType)
            {
                case CombatStateType.PrepareCombat:
                    break;
                case CombatStateType.AllyTurn:

                    OnAllyTurnStarted?.Invoke();

                    if (CurrentMainAlly.CharacterStats.IsStunned)
                    {
                        EndTurn();
                        return;
                    }

                    //GameManager.PersistentGameplayData.CurrentMana = GameManager.PersistentGameplayData.MaxMana;
                    ExecuteUpkeep();

                    CollectionManager.DrawCards(GameManager.PersistentGameplayData.DrawCount);

                    GameManager.PersistentGameplayData.CanSelectCards = true;

                    break;
                case CombatStateType.EnemyTurn:

                    OnEnemyTurnStarted?.Invoke();

                    CollectionManager.DiscardHand();

                    StartCoroutine(nameof(EnemyTurnRoutine));

                    GameManager.PersistentGameplayData.CanSelectCards = false;

                    break;
                case CombatStateType.EndCombat:

                    GameManager.PersistentGameplayData.CanSelectCards = false;

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetStateType), targetStateType, null);
            }
        }

        private void ExecuteUpkeep()
        {
            foreach (AllyBase ally in CurrentAlliesList)
            {
                if (ally.GetType() == typeof(PowerStation))
                {
                    PowerStation powerStation = (PowerStation)ally;

                    GameManager.PersistentGameplayData.CurrentMana = Math.Clamp(
                        GameManager.PersistentGameplayData.CurrentMana + powerStation.Grid,
                        0,
                        GameManager.PersistentGameplayData.MaxMana);
                }
            }
        }
        #endregion

        #region Public Methods
        public void EndTurn()
        {
            StartCoroutine(nameof(AllyTurnRoutine));

            //CurrentCombatStateType = CombatStateType.EnemyTurn;
        }
        public void OnAllyDeath(AllyBase targetAlly)
        {
            var targetAllyData = GameManager.PersistentGameplayData.AllyList.Find(x =>
                x.AllyCharacterData.CharacterID == targetAlly.AllyCharacterData.CharacterID);
            if (GameManager.PersistentGameplayData.AllyList.Count>1)
                GameManager.PersistentGameplayData.AllyList.Remove(targetAllyData);
            CurrentAlliesList.Remove(targetAlly);
            UIManager.InformationCanvas.ResetCanvas();
            if (CurrentAlliesList.Count<=0)
                LoseCombat();
        }
        public void OnEnemyDeath(EnemyBase targetEnemy)
        {
            CurrentEnemiesList.Remove(targetEnemy);
            if (CurrentEnemiesList.Count<=0)
                WinCombat();
        }
        public void DeactivateCardHighlights()
        {
            foreach (var currentEnemy in CurrentEnemiesList)
                currentEnemy.EnemyCanvas.SetHighlight(false);

            foreach (var currentAlly in CurrentAlliesList)
                currentAlly.AllyCanvas.SetHighlight(false);
        }
        public void IncreaseMana(int target)
        {
            GameManager.PersistentGameplayData.CurrentMana += target;
            UIManager.CombatCanvas.SetPileTexts();
        }
        public void HighlightCardTarget(ActionTargetType targetTypeTargetType)
        {
            switch (targetTypeTargetType)
            {
                case ActionTargetType.Enemy:
                    foreach (var currentEnemy in CurrentEnemiesList)
                        currentEnemy.EnemyCanvas.SetHighlight(true);
                    break;
                case ActionTargetType.Ally:
                    foreach (var currentAlly in CurrentAlliesList)
                        currentAlly.AllyCanvas.SetHighlight(true);
                    break;
                case ActionTargetType.AllEnemies:
                    foreach (var currentEnemy in CurrentEnemiesList)
                        currentEnemy.EnemyCanvas.SetHighlight(true);
                    break;
                case ActionTargetType.AllAllies:
                    foreach (var currentAlly in CurrentAlliesList)
                        currentAlly.AllyCanvas.SetHighlight(true);
                    break;
                case ActionTargetType.RandomEnemy:
                    foreach (var currentEnemy in CurrentEnemiesList)
                        currentEnemy.EnemyCanvas.SetHighlight(true);
                    break;
                case ActionTargetType.RandomAlly:
                    foreach (var currentAlly in CurrentAlliesList)
                        currentAlly.AllyCanvas.SetHighlight(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetTypeTargetType), targetTypeTargetType, null);
            }
        }

        private void SpawnAlien()
        {
            var clone = Instantiate(alienPrefab, alienSpawnPos, Quaternion.identity);
            clone.TacticalGrid = tacticalGrid;
            clone.BuildCharacter();
            CurrentEnemiesList.Add(clone);
        }

        public void QueueAlienSpawn(Vector3 spawnPos)
        {
            alienSpawnQueued = true;
            alienSpawnPos = spawnPos;
            //alientTargetIndex = targetIndex;
        }

        public void SpawnSoldier(Vector3 spawnPos)
        {
            var clone = Instantiate(soldierPrefab, spawnPos, Quaternion.identity);
            clone.TacticalGrid = tacticalGrid;
            clone.BuildCharacter();
            CurrentAlliesList.Add(clone);
        }

        public void SpawnPowerStation(Vector3 spawnPos)
        {
            var clone = Instantiate(powerStationPrefab, spawnPos, Quaternion.identity);
            clone.TacticalGrid = tacticalGrid;
            clone.BuildCharacter();
            CurrentAlliesList.Add(clone);
        }

        #endregion

        #region Private Methods
        private void BuildEnemies()
        {
            CurrentEncounter = GameManager.EncounterData.GetEnemyEncounter(
                GameManager.PersistentGameplayData.CurrentStageId,
                GameManager.PersistentGameplayData.CurrentEncounterId,
                GameManager.PersistentGameplayData.IsFinalEncounter);

            var enemyList = CurrentEncounter.EnemyList;
            for (var i = 0; i < enemyList.Count; i++)
            {
                var clone = Instantiate(enemyList[i].EnemyPrefab, EnemyPosList.Count >= i ? EnemyPosList[i] : EnemyPosList[0]);
                clone.TacticalGrid = tacticalGrid;
                clone.BuildCharacter();
                CurrentEnemiesList.Add(clone);
            }
        }
        private void BuildAllies()
        {
            for (var i = 0; i < GameManager.PersistentGameplayData.AllyList.Count; i++)
            {
                var clone = Instantiate(GameManager.PersistentGameplayData.AllyList[i], AllyPosList.Count >= i ? AllyPosList[i] : AllyPosList[0]);
                clone.TacticalGrid = tacticalGrid;
                clone.BuildCharacter();
                CurrentAlliesList.Add(clone);
            }
        }
        private void LoseCombat()
        {
            if (CurrentCombatStateType == CombatStateType.EndCombat) return;

            CurrentCombatStateType = CombatStateType.EndCombat;

            CollectionManager.DiscardHand();
            CollectionManager.DiscardPile.Clear();
            CollectionManager.DrawPile.Clear();
            CollectionManager.HandPile.Clear();
            CollectionManager.HandController.hand.Clear();
            UIManager.CombatCanvas.gameObject.SetActive(true);
            UIManager.CombatCanvas.CombatLosePanel.SetActive(true);
        }
        private void WinCombat()
        {
            if (CurrentCombatStateType == CombatStateType.EndCombat) return;

            CurrentCombatStateType = CombatStateType.EndCombat;

            foreach (var allyBase in CurrentAlliesList)
            {
                GameManager.PersistentGameplayData.SetAllyHealthData(allyBase.AllyCharacterData.CharacterID,
                    allyBase.CharacterStats.CurrentHealth, allyBase.CharacterStats.MaxHealth);
            }

            CollectionManager.ClearPiles();


            if (GameManager.PersistentGameplayData.IsFinalEncounter)
            {
                UIManager.CombatCanvas.CombatWinPanel.SetActive(true);
            }
            else
            {
                CurrentMainAlly.CharacterStats.ClearAllStatus();
                GameManager.PersistentGameplayData.CurrentEncounterId++;
                UIManager.CombatCanvas.gameObject.SetActive(false);
                UIManager.RewardCanvas.gameObject.SetActive(true);
                UIManager.RewardCanvas.PrepareCanvas();
                UIManager.RewardCanvas.BuildReward(RewardType.Gold);
                UIManager.RewardCanvas.BuildReward(RewardType.Card);
            }

        }
        #endregion

        #region Routines
        private IEnumerator EnemyTurnRoutine()
        {
            var waitDelay = new WaitForSeconds(0.2f);

            yield return new WaitForSeconds(0.5f);

            foreach (var currentEnemy in CurrentEnemiesList)
            {
                //yield return currentEnemy.StartCoroutine(nameof(EnemyExample.ActionRoutine));
                yield return currentEnemy.ActionRoutine();
                yield return waitDelay;
            }

            if (alienSpawnQueued)
            {
                yield return new WaitForSeconds(0.5f);
                SpawnAlien();
                alienSpawnQueued = false;
            }

            if (CurrentCombatStateType != CombatStateType.EndCombat)
                CurrentCombatStateType = CombatStateType.AllyTurn;
        }

        private IEnumerator AllyTurnRoutine()
        {
            var waitDelay = new WaitForSeconds(0.1f);

            foreach (var currentAlly in CurrentAlliesList)
            {
                yield return currentAlly.AttackRoutine();
                yield return waitDelay;
            }

            yield return new WaitForSeconds(0.2f);

            if (CurrentCombatStateType != CombatStateType.EndCombat)
                CurrentCombatStateType = CombatStateType.EnemyTurn;
        }
        #endregion
    }
}