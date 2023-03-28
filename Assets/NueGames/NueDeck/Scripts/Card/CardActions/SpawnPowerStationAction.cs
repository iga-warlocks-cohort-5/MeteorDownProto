using Grids2D;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class SpawnPowerStationAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.SpawnPowerStation;
        public override void DoAction(CardActionParameters actionParameters)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0.0f;

            Cell targetCell = CombatManager.TacticalGrid.CellGetAtPosition(mousePos, true);

            if (targetCell.tag == 0)
            {
                CombatManager.SpawnPowerStation(CombatManager.TacticalGrid.CellGetPosition(targetCell.index));
            }
        }
    }
}