using System;
using System.Collections;
using Assets.Scripts;
using Grids2D;
using NueGames.NueDeck.Scripts.Data.Characters;
using NueGames.NueDeck.Scripts.Interfaces;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Characters
{
    public abstract class AllyBase : CharacterBase,IAlly
    {
        [Header("Ally Base Settings")]
        [SerializeField] private AllyCanvas allyCanvas;
        [SerializeField] private AllyCharacterData allyCharacterData;
        public AllyCanvas AllyCanvas => allyCanvas;
        public AllyCharacterData AllyCharacterData => allyCharacterData;

        public Grid2D TacticalGrid;

        public CellTags cellTag { get; protected set; }

        public override void BuildCharacter()
        {
            base.BuildCharacter();
            allyCanvas.InitCanvas();
            CharacterStats = new CharacterStats(allyCharacterData.MaxHealth,allyCanvas);

            if (!GameManager)
                throw new Exception("There is no GameManager");

            var data = GameManager.PersistentGameplayData.AllyHealthDataList.Find(x =>
                x.CharacterId == AllyCharacterData.CharacterID);

            if (data != null)
            {
                CharacterStats.CurrentHealth = data.CurrentHealth;
                CharacterStats.MaxHealth = data.MaxHealth;
            }
            else
            {
                GameManager.PersistentGameplayData.SetAllyHealthData(AllyCharacterData.CharacterID,CharacterStats.CurrentHealth,CharacterStats.MaxHealth);
            }

            CharacterStats.OnDeath += OnDeath;
            CharacterStats.SetCurrentHealth(CharacterStats.CurrentHealth);

            if (CombatManager != null)
                CombatManager.OnAllyTurnStarted += CharacterStats.TriggerAllStatus;
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            TacticalGrid.CellSetCanCross(TacticalGrid.CellGetAtPosition(transform.position, true).index, true);
            TacticalGrid.CellSetTag(TacticalGrid.CellGetAtPosition(transform.position, true), 0);

            if (CombatManager != null)
            {
                CombatManager.OnAllyTurnStarted -= CharacterStats.TriggerAllStatus;
                CombatManager.OnAllyDeath(this);
            }

            Destroy(gameObject);
        }

        public virtual IEnumerator AttackRoutine()
        {
            yield return null;
        }
    }

    [Serializable]
    public class AllyHealthData
    {
        [SerializeField] private string characterId;
        [SerializeField] private int maxHealth;
        [SerializeField] private int currentHealth;

        public int MaxHealth
        {
            get => maxHealth;
            set => maxHealth = value;
        }

        public int CurrentHealth
        {
            get => currentHealth;
            set => currentHealth = value;
        }

        public string CharacterId
        {
            get => characterId;
            set => characterId = value;
        }
    }
}