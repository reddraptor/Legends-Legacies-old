using UnityEngine;
using Assets.Scripts.Components;

namespace Assets.Scripts.Data_Types
{
    public class Movement
    {
        internal Movable movable;
        internal Vector2 startPosition = Vector2.zero;
        internal Vector2 direction = Vector2.zero;
        internal float speed = 0f;

        public Movement(Movable moveable, Vector2 startPosition, Vector2 direction, float speed)
        {
            this.movable = moveable;  this.startPosition = startPosition; this.direction = direction; this.speed = speed;
        }

        public void Add(Vector2 addDirection, float addSpeed)
        {
            Vector2 speedVector = direction.normalized * speed;
            Vector2 addSpeedVector = addDirection.normalized * addSpeed;

            speed = (speedVector + addSpeedVector).magnitude;
            direction += addDirection;
        }

        public Vector2 EndPosition
        {
            get { return startPosition + direction; }
        }

        public override string ToString()
        {
            return "Entity: " + movable.entity + "; Start: " + startPosition + "; Direction: " + direction + "; Speed: " + speed;
        }
    }

}