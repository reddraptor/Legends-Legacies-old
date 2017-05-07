using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.Components;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.ScriptableObjects.BehaviorScripts
{
    public abstract class BehaviorScript : ScriptableObject
    {
        public enum Direction { North, Northeast, East, Southeast, South, Southwest, West, Northwest}

        public abstract void Run(BehaviorsManager behaviorManager, Behaviors behaviors);

        public Vector2 DirectionVector(Direction direction)
        {
            Vector2 directionVector = new Vector2();

            if (direction == Direction.North)
            {
                directionVector.x = 0; directionVector.y = 1;
            }
            else if (direction == Direction.Northeast)
            {
                directionVector.x = 1; directionVector.y = 1;
            }
            else if (direction == Direction.East)
            {
                directionVector.x = 1; directionVector.y = 0;
            }
            else if (direction == Direction.Southeast)
            {
                directionVector.x = 1; directionVector.y = -1;
            }
            else if (direction == Direction.South)
            {
                directionVector.x = 0; directionVector.y = -1;
            }
            else if (direction == Direction.Southwest)
            {
                directionVector.x = -1; directionVector.y = -1;
            }
            else if (direction == Direction.West)
            {
                directionVector.x = -1; directionVector.y = 0;
            }
            else if (direction == Direction.Northwest)
            {
                directionVector.x = -1; directionVector.y = 1;
            }

            return directionVector;
        }

        public Direction RandomDirection(System.Random random)
        {
            return (Direction) random.Next(8);
        }

        public bool Roll(System.Random random, float percent)
        {
            return random.NextDouble() < percent ? true : false;
        }

        private void OnEnable()
        {
        }
    }
}
