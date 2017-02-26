using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;

namespace Assets.Scripts.ScriptableObjects.BehaviorScripts
{
    [CreateAssetMenu(menuName = "LegendsLegacies/Behaviors/Idle")]
    public class IdleBehavior : BehaviorScript
    {
        [Range(0f, 1.0f)]
        public float idleMovementPercentage = 0.25f;

        public override void Run(BehaviorManager behaviorManager)
        {
            bool makeMove = Roll(idleMovementPercentage);
            IntegerPair unitVector;

            if (makeMove)
            {
                unitVector = UnitVector(RandomDirection());

                if (behaviorManager.movementManager && behaviorManager.entityManager)
                {
                    int newX = behavior.entity.coordinates.World.X + unitVector.horizontal;
                    int newY = behavior.entity.coordinates.World.Y + unitVector.vertical;

                    behaviorManager.entityManager.Place(behavior.entity, new Coordinates(newX, newY));
                    behaviorManager.movementManager.Add(behavior.movement, unitVector.horizontal, unitVector.vertical, behavior.GetComponent<Mob>().speed);

                }
            }

        }

    }
}
