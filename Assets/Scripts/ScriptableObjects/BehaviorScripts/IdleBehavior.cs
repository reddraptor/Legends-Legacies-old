﻿using UnityEngine;
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

        public override void Run(BehaviorsManager behaviorManager, Behaviors behaviors)
        {
            bool makeMove = Roll(behaviorManager.randomizer, idleMovementPercentage);
            Vector2 directionVector;

            if (makeMove & (behaviors.state == Behaviors.State.Idle) )
            {
                directionVector = DirectionVector(RandomDirection(behaviorManager.randomizer));
                if (behaviorManager && behaviors) 
                    if (behaviorManager.Move(behaviors.entityMember, directionVector))
                        behaviors.state = Behaviors.State.Moving;
            }
        }
    }
}
