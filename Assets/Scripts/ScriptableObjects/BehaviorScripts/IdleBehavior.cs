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
            //DEBUG
            idleMovementPercentage = 1;
            //END DEBUG
            bool makeMove = Roll(behaviorManager.randomizer, idleMovementPercentage);
            Vector2 directionVector;

            if (makeMove & behavior.isIdle)
            {
                behavior.isIdle = false;
                directionVector = DirectionVector(RandomDirection(behaviorManager.randomizer));

                if (behaviorManager.entityManager)
                {
                    behaviorManager.entityManager.Move(behavior.entity, directionVector);
                }
            }

        }

    }
}
