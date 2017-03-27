using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(Entity))]
    public class Movement : MonoBehaviour
    {
        /* EDITOR FIELDS */
        public float speed = 0f;

        internal bool isActive = false;
        internal Vector2 startPosition = Vector2.zero;
        internal Vector2 vector = Vector2.zero; //representing direction, with magnitude designating distance

        internal Entity Entity
        {
            get { return GetComponent<Entity>(); }
        }

        /* PRIVATE FIELDS */
        
        internal Coordinates Coordinates
        {
            get { return Entity.Coordinates; }
        }

        internal new Rigidbody2D rigidbody;


        /* UNITY MESSAGES */
        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override string ToString()
        {
            return "Entity: " + Entity + "; Start: " + startPosition + "; Vector: " + vector + "; Speed: " + speed;

        }
    }

}