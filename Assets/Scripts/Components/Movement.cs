using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(Entity))]
    public class Movement : MonoBehaviour
    {
        /* EDITOR FIELDS */
        //public float speed = 2;
        public float speed = 2f;
        public int horizontal = 0;
        public int vertical = 0;
        public bool isMoving = false;
        
        /* PRIVATE FIELDS */
        Rigidbody2D rb;
        Entity entity;


        public Coordinates coordinates
        {
            get { return entity.coordinates; }
        }

        public Rigidbody2D Rigidbody
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
            entity = GetComponent<Entity>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}