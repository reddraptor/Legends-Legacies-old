using UnityEngine;
using Assets.Scripts.Data_Types;
using System.Collections.Generic;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movable : MonoBehaviour
    {
        public override string ToString()
        {
            string output = "Entity: " + entity + "\n";

            if (destinationVector != null)
                output += "Destination vector: " + destinationVector + "\n";
            else
                output += "Destination vector: null";
            output += "Speed: " + speed + "\n";

            return output;
        }

        public override int GetHashCode()
        {
            return entity.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other is Movable)
            {
                return (((Movable)other).GetHashCode() == GetHashCode());
            }
            else return false;
        }

        public Coordinates Coordinates
        {
            get { return entity.Coordinates; }
        }

        public bool IsMoving { get { return moving; } }

        public Entity Entity { get { return entity; } }

        public Rigidbody2D RigidBody { get { return rigidbody; } }


        internal bool moving;
        internal Vector2 destinationVector = Vector2.zero;
        internal float speed = 0f;
        
        private Entity entity;
        private new Rigidbody2D rigidbody;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            entity = GetComponent<Entity>();
        }
    }

}