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
            bool makeMove = Roll(behaviorManager.randomizer, idleMovementPercentage);
            IntegerPair unitVector;

            if (makeMove)
            {
                unitVector = UnitVector(RandomDirection(behaviorManager.randomizer));

                if (behaviorManager.movementManager && behaviorManager.entityManager)
                {
                    long newX = behavior.entity.coordinates.inWorld.x + unitVector.horizontal;
                    long newY = behavior.entity.coordinates.inWorld.y + unitVector.vertical;

                    if (behaviorManager.entityManager.Place(behavior.entity, new Coordinates(newX, newY)))
                    {
                        float speed = behaviorManager.worldManager.GetSpeed(behavior.entity.coordinates, behavior.attributes, TerrainTile.TerrainType.Land);
                        behaviorManager.movementManager.Add(behavior.movement, unitVector.horizontal, unitVector.vertical, speed);
                    }

                }
            }

        }

    }
}
