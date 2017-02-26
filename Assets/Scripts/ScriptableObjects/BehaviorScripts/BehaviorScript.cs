using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.Components;

namespace Assets.Scripts.ScriptableObjects.BehaviorScripts
{
    public abstract class BehaviorScript : ScriptableObject
    {
        public enum Direction { North, Northeast, East, Southeast, South, Southwest, West, Northwest}

        public Behavior behavior;

        private System.Random random;

        public abstract void Run(BehaviorManager behaviorManager);

        public IntegerPair UnitVector(Direction direction)
        {
            IntegerPair unitVector = new IntegerPair();

            if (direction == Direction.North)
            {
                unitVector.horizontal = 0; unitVector.vertical = 1;
            }
            else if (direction == Direction.Northeast)
            {
                unitVector.horizontal = 1; unitVector.vertical = 1;
            }
            else if (direction == Direction.East)
            {
                unitVector.horizontal = 1; unitVector.vertical = 0;
            }
            else if (direction == Direction.Southeast)
            {
                unitVector.horizontal = 1; unitVector.vertical = -1;
            }
            else if (direction == Direction.South)
            {
                unitVector.horizontal = 0; unitVector.vertical = -1;
            }
            else if (direction == Direction.Southwest)
            {
                unitVector.horizontal = -1; unitVector.vertical = -1;
            }
            else if (direction == Direction.West)
            {
                unitVector.horizontal = -1; unitVector.vertical = 0;
            }
            else if (direction == Direction.Northwest)
            {
                unitVector.horizontal = -1; unitVector.vertical = 1;
            }

            return unitVector;
        }

        public Direction RandomDirection()
        {
            return (Direction) random.Next(8);
        }

        public bool Roll(float percent)
        {
            return random.NextDouble() < percent ? true : false;
        }

        private void OnEnable()
        {
            random = new System.Random();
        }
    }
}
