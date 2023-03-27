using Grids2D;
using NueGames.NueDeck.Scripts.Characters.Enemies;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.EnemyBehaviour.EnemyActions
{
    public class WaitAction : EnemyActionBase
    {
        public override EnemyActionType ActionType => EnemyActionType.Wait;

        public override void DoAction(EnemyActionParameters actionParameters)
        {
            // Do nothing
        }
    }
}