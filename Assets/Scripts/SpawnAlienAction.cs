using Grids2D;
using NueGames.NueDeck.Scripts.Characters.Enemies;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.EnemyBehaviour.EnemyActions
{
    public class SpawnAlienAction : EnemyActionBase
    {
        public override EnemyActionType ActionType => EnemyActionType.SpawnAlien;

        public override void DoAction(EnemyActionParameters actionParameters)
        {
            Vector3 podPosition = actionParameters.SelfCharacter.transform.position;
            Grid2D tacticalGrid = (actionParameters.SelfCharacter as AlienPod).TacticalGrid;
            Cell podCell = tacticalGrid.CellGetAtPosition(podPosition, true);

            if (tacticalGrid.CellGetAtPosition(podCell.column, podCell.row - 1).tag == 0)
            {
                //CombatManager.QueueAlienSpawn(podPosition, tacticalGrid.CellGetAtPosition(podCell.column, podCell.row - 1).index);
                //Vector3 spawnPos = tacticalGrid.CellGetPosition(podCell.column, podCell.row - 1);

                Vector3 spawnPos = tacticalGrid.CellGetPosition(tacticalGrid.CellGetAtPosition(podCell.column, podCell.row - 1).index);

                CombatManager.QueueAlienSpawn(spawnPos);
            }
            else if (tacticalGrid.CellGetAtPosition(podCell.column + 1, podCell.row - 1).tag == 0)
            {
                //CombatManager.QueueAlienSpawn(podPosition, tacticalGrid.CellGetAtPosition(podCell.column + 1, podCell.row - 1).index);
                Vector3 spawnPos = tacticalGrid.CellGetPosition(tacticalGrid.CellGetAtPosition(podCell.column + 1, podCell.row - 1).index);

                CombatManager.QueueAlienSpawn(spawnPos);
            }
            else if (tacticalGrid.CellGetAtPosition(podCell.column + 1, podCell.row).tag == 0)
            {
                //CombatManager.QueueAlienSpawn(podPosition, tacticalGrid.CellGetAtPosition(podCell.column + 1, podCell.row).index);
                Vector3 spawnPos = tacticalGrid.CellGetPosition(tacticalGrid.CellGetAtPosition(podCell.column + 1, podCell.row).index);

                CombatManager.QueueAlienSpawn(spawnPos);
            }
            else if (tacticalGrid.CellGetAtPosition(podCell.column + 1, podCell.row + 1).tag == 0)
            {
                //CombatManager.QueueAlienSpawn(podPosition, tacticalGrid.CellGetAtPosition(podCell.column + 1, podCell.row).index);
                Vector3 spawnPos = tacticalGrid.CellGetPosition(tacticalGrid.CellGetAtPosition(podCell.column + 1, podCell.row + 1).index);

                CombatManager.QueueAlienSpawn(spawnPos);
            }
            else if (tacticalGrid.CellGetAtPosition(podCell.column, podCell.row + 1).tag == 0)
            {
                //CombatManager.QueueAlienSpawn(podPosition, tacticalGrid.CellGetAtPosition(podCell.column + 1, podCell.row).index);
                Vector3 spawnPos = tacticalGrid.CellGetPosition(tacticalGrid.CellGetAtPosition(podCell.column, podCell.row + 1).index);

                CombatManager.QueueAlienSpawn(spawnPos);
            }
            else if (tacticalGrid.CellGetAtPosition(podCell.column - 1, podCell.row + 1).tag == 0)
            {
                //CombatManager.QueueAlienSpawn(podPosition, tacticalGrid.CellGetAtPosition(podCell.column + 1, podCell.row).index);
                Vector3 spawnPos = tacticalGrid.CellGetPosition(tacticalGrid.CellGetAtPosition(podCell.column - 1, podCell.row + 1).index);

                CombatManager.QueueAlienSpawn(spawnPos);
            }
            else if (tacticalGrid.CellGetAtPosition(podCell.column - 1, podCell.row).tag == 0)
            {
                //CombatManager.QueueAlienSpawn(podPosition, tacticalGrid.CellGetAtPosition(podCell.column + 1, podCell.row).index);
                Vector3 spawnPos = tacticalGrid.CellGetPosition(tacticalGrid.CellGetAtPosition(podCell.column - 1, podCell.row).index);

                CombatManager.QueueAlienSpawn(spawnPos);
            }
            else if (tacticalGrid.CellGetAtPosition(podCell.column - 1, podCell.row - 1).tag == 0)
            {
                //CombatManager.QueueAlienSpawn(podPosition, tacticalGrid.CellGetAtPosition(podCell.column + 1, podCell.row).index);
                Vector3 spawnPos = tacticalGrid.CellGetPosition(tacticalGrid.CellGetAtPosition(podCell.column - 1, podCell.row - 1).index);

                CombatManager.QueueAlienSpawn(spawnPos);
            }
        }
    }
}