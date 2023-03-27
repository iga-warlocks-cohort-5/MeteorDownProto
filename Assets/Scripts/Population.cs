using NueGames.NueDeck.Scripts.Managers;
using Assets.Scripts;

namespace NueGames.NueDeck.Scripts.Characters.Allies
{
    public class Population : AllyBase
    {
        public override void BuildCharacter()
        {
            base.BuildCharacter();

            cellTag = CellTags.Population;

            TacticalGrid.CellSetCanCross(TacticalGrid.CellGetAtPosition(transform.position, true).index, false);
            TacticalGrid.CellSetTag(TacticalGrid.CellGetAtPosition(transform.position, true), (int)cellTag);

            if (UIManager != null)
                CharacterStats.OnHealthChanged += UIManager.InformationCanvas.SetHealthText;
            CharacterStats.SetCurrentHealth(CharacterStats.CurrentHealth);
        }
    }
}