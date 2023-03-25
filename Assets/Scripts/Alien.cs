using NueGames.NueDeck.Scripts.Data.Characters;
using NueGames.NueDeck.Scripts.EnemyBehaviour;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.NueExtentions;
using System.Collections;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Characters.Enemies
{
    public class Alien : EnemyBase
    {
        protected override IEnumerator AttackRoutine(EnemyAbilityData targetAbility)
        {
            var waitFrame = new WaitForEndOfFrame();

            if (CombatManager == null) yield break;

            //var target = CombatManager.CurrentAlliesList.RandomItem();

            CharacterBase target = null;

            foreach (CharacterBase ally in CombatManager.CurrentAlliesList)
            {
                if (ally.gameObject.tag == "PowerStation")
                {
                    target = ally;

                    break;
                }
            }

            if (target == null)
            {
                foreach (CharacterBase ally in CombatManager.CurrentAlliesList)
                {
                    if (ally.gameObject.tag == "Population")
                    {
                        target = ally;
                    }
                }
            }

            if (target == null) yield break;

            var startPos = transform.position;
            var endPos = target.transform.position;

            var startRot = transform.localRotation;
            var endRot = Quaternion.Euler(60, 0, 60);

            yield return StartCoroutine(MoveToTargetRoutine(waitFrame, startPos, endPos, startRot, endRot, 5));

            targetAbility.ActionList.ForEach(x => EnemyActionProcessor.GetAction(x.ActionType).DoAction(new EnemyActionParameters(x.ActionValue, target, this)));

            yield return StartCoroutine(MoveToTargetRoutine(waitFrame, endPos, startPos, endRot, startRot, 5));
        }
    }
}