using NueGames.NueDeck.Scripts.Managers;
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
            if (UIManager != null)
                CharacterStats.OnHealthChanged += UIManager.InformationCanvas.SetHealthText;
            CharacterStats.SetCurrentHealth(CharacterStats.CurrentHealth);
        }
    }
}

