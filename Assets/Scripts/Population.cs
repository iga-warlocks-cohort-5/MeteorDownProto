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

            if (UIManager != null)
                CharacterStats.OnHealthChanged += UIManager.InformationCanvas.SetHealthText;
            CharacterStats.SetCurrentHealth(CharacterStats.CurrentHealth);
        }
    }
}