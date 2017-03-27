using UnityEngine;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Components
{
    [DisallowMultipleComponent]
    public class Entity : MonoBehaviour
    {
        /* INSPECTOR FIELDS */

        /* PUBLIC FIELDS */
        //public Coordinates coordinates = new Coordinates(0, 0);
        public Chunk chunk = null;
        public IntegerPair tileIndices = new IntegerPair(0, 0);

        public Coordinates Coordinates
        {
            get
            {
                if (chunk != null)
                    return new Coordinates(chunk, tileIndices);
                else
                    return new Coordinates();
            }
        }

        public bool Placed
        {
            get { return placed; }
            set
            {
                if (value)
                {
                    gameObject.layer = LayerMask.GetMask("Default");
                    Show = true;
                    placed = true;
                }
                else
                {
                    gameObject.layer = LayerMask.GetMask("Ignore Raycast");
                    Show = false;
                    placed = false;
                }
            }
        }

        public Vector2 Position
        {
            get
            {
                if (GetComponent<Rigidbody2D>())
                    return GetComponent<Rigidbody2D>().position;
                else return Vector2.zero;
            }
            set
            {
                if (GetComponent<Rigidbody2D>())
                    GetComponent<Rigidbody2D>().position = value;
            }
        }

        public string Type
        {
            get { return tag; }
        }

        public Movement Movement
        {
            get { return GetComponent<Movement>(); }
        }

        public Attributes Attributes
        {
            get { return GetComponent<Attributes>(); }
        }

        public bool IsCentered
        {
            get
            {
                if (transform.position == Vector3.zero) return true;
                else return false;
            }
        }

        public bool Show 
        {
            get
            {
                if (GetComponent<SpriteRenderer>())
                    return GetComponent<SpriteRenderer>().enabled;
                else return false;
            }
            set
            {
                if (GetComponent<SpriteRenderer>())
                    GetComponent<SpriteRenderer>().enabled = value;
            }
        }

        private bool placed = false;

        public override string ToString()
        {
            return name + GetInstanceID() + " (" + Type + ")";
        }

        public override int GetHashCode()
        {
            return GetInstanceID();
        }

        public override bool Equals(object other)
        {
            if (other is Entity)
            {
                if (this.GetInstanceID() == ((Entity)other).GetInstanceID()) return true;
            }
            return false;
        }

    }
}