using NueGames.NueDeck.Scripts.Managers;
using Assets.Scripts;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Characters.Allies
{
    public class PowerStation : AllyBase
    {
        public int Grid => _grid;

        [SerializeField]
        private int _grid = 10;

        public override void BuildCharacter()
        {
            base.BuildCharacter();

            cellTag = CellTags.PowerStation;

            TacticalGrid.CellSetCanCross(TacticalGrid.CellGetAtPosition(transform.position, true).index, false);
            TacticalGrid.CellSetTag(TacticalGrid.CellGetAtPosition(transform.position, true), (int)cellTag);

            if (UIManager != null)
                CharacterStats.OnHealthChanged += UIManager.InformationCanvas.SetHealthText;
            CharacterStats.SetCurrentHealth(CharacterStats.CurrentHealth);
        }
    }
}

