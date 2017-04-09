using UnityEngine;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Components
{
    [DisallowMultipleComponent]
    public class Entity : MonoBehaviour
    {

        public Chunk Chunk
        {
            get { return chunk; }
        }

        public IntegerPair TileIndices
        {
            get { return tileIndices; }
        }

        public EntityCollection ParentCollection
        {
            get
            {
                if (transform.parent)
                    return transform.parent.GetComponent<EntityCollection>();
                else return null;
            }
        }


        public Coordinates Coordinates
        {
            get
            {
                if (Chunk != null)
                    return new Coordinates(Chunk, TileIndices);
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
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        public string Type
        {
            get { return tag; }
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

        public void SetLocation(Chunk newChunk, IntegerPair newIndices)
        {
            if (ParentCollection)
            {
                ParentCollection.UpdateLocation(this, new Coordinates(newChunk, newIndices));
            }
            chunk = newChunk;
            tileIndices = newIndices;
        }


        private Chunk chunk = null;
        private IntegerPair tileIndices = new IntegerPair(0, 0);
        private bool placed = false;

        protected virtual void Start()
        {
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("Collision!" + this + " and " + collision.collider.GetComponent<Entity>());
        }
    }
}