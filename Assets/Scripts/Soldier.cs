using Assets.Scripts;
using NueGames.NueDeck.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using NueGames.NueDeck.Scripts.Characters.Enemies;

namespace NueGames.NueDeck.Scripts.Characters.Allies
{
    public class Soldier : AllyBase
    {
        [SerializeField]
        private int attackDamage = 1;

        private CharacterBase target = null;

        public override void BuildCharacter()
        {
            base.BuildCharacter();

            cellTag = CellTags.Soldier;

            if (UIManager != null)
                CharacterStats.OnHealthChanged += UIManager.InformationCanvas.SetHealthText;
            CharacterStats.SetCurrentHealth(CharacterStats.CurrentHealth);
        }

        private void SelectTarget()
        {
            foreach (CharacterBase enemy in CombatManager.CurrentEnemiesList)
            {
                if (enemy.gameObject.tag == "Alien")
                {
                    target = enemy;

                    break;
                }
            }

            if (target == null)
            {
                foreach (CharacterBase enemy in CombatManager.CurrentEnemiesList)
                {
                    if (enemy.gameObject.tag == "AlienPod")
                    {
                        target = enemy;

                        break;
                    }
                }
            }
        }

        public override IEnumerator AttackRoutine()
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
                if (target.GetType() == typeof(Alien) && TacticalGrid.cells[index].tag == (int)CellTags.Alien)
                {
                    targetFound = true;

                    break;
                }
                else if (target.GetType() == typeof(AlienPod) && TacticalGrid.cells[index].tag == (int)CellTags.AlienPod)
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

                target.CharacterStats.Damage(attackDamage);

                yield return StartCoroutine(MoveToTargetRoutine(waitFrame, endPos, startPos, endRot, startRot, 5));
            }
            else
            {
                List<int> path = TacticalGrid.FindPath(startIndex, endIndex);

                TacticalGrid.MoveTo(gameObject, path[0], 5.0f);
                TacticalGrid.CellSetTag(startIndex, 0);
                TacticalGrid.CellSetTag(path[0], (int)CellTags.Soldier);
                startIndex = path[0];
            }

            TacticalGrid.CellSetCanCross(startIndex, false);
            TacticalGrid.CellSetCanCross(endIndex, false);
        }

        protected IEnumerator MoveToTargetRoutine(WaitForEndOfFrame waitFrame, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, float speed)
        {
            var timer = 0f;
            while (true)
            {
                timer += Time.deltaTime * speed;

                transform.position = Vector3.Lerp(startPos, endPos, timer);
                transform.localRotation = Quaternion.Lerp(startRot, endRot, timer);
                if (timer >= 1f)
                {
                    break;
                }

                yield return waitFrame;
            }
        }
    }
}