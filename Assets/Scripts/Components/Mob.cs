using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{

    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(Entity))]

    class Mob : MonoBehaviour
    {

        public float speed = 5;

        public Coordinates coordinates
        {
            get { return _entity.coordinates; }
        }

        public Entity entity
        {
            get { return _entity; }
        }

        public Movement movement
        {
            get { return _movement; }
        }

        /* PRIVATE FIELDS */
        Entity _entity;
        Movement _movement;

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            name = "cow";
            _entity = GetComponent<Entity>();
            _entity.type = EntityManager.EntityType.Mob;
            _movement = GetComponent<Movement>();
        }

    }
}
