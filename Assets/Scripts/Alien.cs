using Assets.Scripts;
using Grids2D;
using NueGames.NueDeck.Scripts.Characters.Allies;
using NueGames.NueDeck.Scripts.Data.Characters;
using NueGames.NueDeck.Scripts.EnemyBehaviour;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.NueExtentions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Characters.Enemies
{
    public class Alien : EnemyBase
    {
        private CharacterBase target = null;

        public override void BuildCharacter()
        {
            base.BuildCharacter();

            cellTag = CellTags.Alien;
        }

        private void SelectTarget()
        {
            foreach (CharacterBase ally in CombatManager.CurrentAlliesList)
            {
                if (ally.gameObject.tag == "Soldier")
                {
                    target = ally;

                    break;
                }
            }

            if (target == null)
            {
                foreach (CharacterBase ally in CombatManager.CurrentAlliesList)
                {
                    if (ally.gameObject.tag == "PowerStation")
                    {
                        target = ally;

                        break;
                    }
                }
            }

            if (target == null)
            {
                foreach (CharacterBase ally in CombatManager.CurrentAlliesList)
                {
                    if (ally.gameObject.tag == "Population")
                    {
                        target = ally;

                        break;
                    }
                }
            }
        }

        protected override IEnumerator AttackRoutine(EnemyAbilityData targetAbility)
        {
            var waitFrame = new WaitForEndOfFrame();

            if (CombatManager == null) yield break;

            SelectTarget();

            if (target == null) yield break;

            int startIndex = TacticalGrid.CellGetAtPosition(transform.position, true).index;
            int endIndex = TacticalGrid.CellGetAtPosition(target.transform.position, true).index;

            TacticalGrid.CellSetCanCross(startIndex, true);
            TacticalGrid.CellSetCanCross(endIndex, true);

            List<int> adjacents = TacticalGrid.CellGetNeighbours(TacticalGrid.CellGetAtPosition(transform.position, true).index, 1);
            bool targetFound = false;

            foreach (int index in adjacents)
            {
                if (target.GetType() == typeof(Soldier) && TacticalGrid.cells[index].tag == (int)CellTags.Soldier)
                {
                    targetFound = true;

                    break;
                }
                else if (target.GetType() == typeof(PowerStation) && TacticalGrid.cells[index].tag == (int)CellTags.PowerStation)
                {
                    targetFound = true;

                    break;
                }
                else if (target.GetType() == typeof(Population) && TacticalGrid.cells[index].tag == (int)CellTags.Population)
                {
                    targetFound = true;

                    break;
                }

                if ((int)(target as AllyBase).cellTag == TacticalGrid.cells[index].tag)
                {
                    targetFound = true;

                    break;
                }
            }

            if (targetFound)
            {
                var startPos = transform.position;
                var endPos = target.transform.position;

                var startRot = transform.localRotation;
                var endRot = Quaternion.Euler(60, 0, 60);

                yield return StartCoroutine(MoveToTargetRoutine(waitFrame, startPos, endPos, startRot, endRot, 5));

                targetAbility.ActionList.ForEach(x => EnemyActionProcessor.GetAction(x.ActionType).DoAction(new EnemyActionParameters(x.ActionValue, target, this)));

                yield return StartCoroutine(MoveToTargetRoutine(waitFrame, endPos, startPos, endRot, startRot, 5));
            }
            else
            {
                List<int> path = TacticalGrid.FindPath(startIndex, endIndex);

                TacticalGrid.MoveTo(gameObject, path[0], 5.0f);
                TacticalGrid.CellSetTag(startIndex, 0);
                TacticalGrid.CellSetTag(path[0], (int)CellTags.Alien);
                startIndex = path[0];
            }

            TacticalGrid.CellSetCanCross(startIndex, false);
            TacticalGrid.CellSetCanCross(endIndex, false);
        }
    }
}