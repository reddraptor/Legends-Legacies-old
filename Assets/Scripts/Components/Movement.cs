using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(Entity))]
    public class Movement : MonoBehaviour
    {
        /* EDITOR FIELDS */
        public float speed = 0f;
        public bool isMoving = false;
        public Vector2 startPosition = Vector2.zero;
        public Vector2 vector = Vector2.zero; //representing direction, with magnitude designating distance

        public Entity entity
        {
            get { return GetComponent<Entity>(); }
        }

        /* PRIVATE FIELDS */
        Rigidbody2D rb;
        

        
        public Coordinates coordinates
        {
            get { return entity.coordinates; }
        }

        public new Rigidbody2D rigidbody
        {
            get { return rb; }
        }

        /* UNITY MESSAGES */
        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}