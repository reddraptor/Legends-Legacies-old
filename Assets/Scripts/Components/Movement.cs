using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(Entity))]
    public class Movement : MonoBehaviour
    {
        /* EDITOR FIELDS */
        public float speed = 0f;
        public int horizontal = 0;
        public int vertical = 0;
        public bool isMoving = false;
        

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