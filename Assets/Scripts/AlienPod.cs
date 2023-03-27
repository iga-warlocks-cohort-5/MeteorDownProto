using Assets.Scripts;
using NueGames.NueDeck.Scripts.Data.Characters;
using NueGames.NueDeck.Scripts.EnemyBehaviour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Characters.Enemies
{
    public class AlienPod : EnemyBase
    {
        public override void BuildCharacter()
        {
            base.BuildCharacter();

            cellTag = CellTags.AlienPod;

            TacticalGrid.CellSetCanCross(TacticalGrid.CellGetAtPosition(transform.position, true).index, false);
            TacticalGrid.CellSetTag(TacticalGrid.CellGetAtPosition(transform.position, true), (int)cellTag);
        }

        protected override IEnumerator AttackRoutine(EnemyAbilityData targetAbility)
        {
            var waitFrame = new WaitForEndOfFrame();

            if (CombatManager == null) yield break;

            targetAbility.ActionList.ForEach(x => EnemyActionProcessor.GetAction(x.ActionType).DoAction(new EnemyActionParameters(x.ActionValue, this, this)));
        }
    }
}
