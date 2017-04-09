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
            string output = entity + "\n";

            if (currentMovement != null)
                output += "Current Move: " + currentMovement + "\n";

            if (nextMovement != null)
                output += "Next Move: " + nextMovement;

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

        public bool IsMoving { get { return moving; } }

        public Coordinates Coordinates
        {
            get { return entity.Coordinates; }
        }
        
        internal Entity entity;
        internal new Rigidbody2D rigidbody;
        internal Movement currentMovement;
        internal Movement nextMovement;
        internal bool moving = false;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            entity = GetComponent<Entity>();
        }
    }

}