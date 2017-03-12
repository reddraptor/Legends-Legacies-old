using UnityEngine;
using Assets.Scripts.ScriptableObjects.BehaviorScripts;

namespace Assets.Scripts.Components
{
    public class Behavior: MonoBehaviour
    {
        public IdleBehavior idlePrefab;

        IdleBehavior iBehavior;

        public IdleBehavior idleBehavior
        {
            get { return iBehavior; }
        }

        public Movement movement {
            get { return GetComponent<Movement>(); }
        }

        public Entity entity
        {
            get { return GetComponent<Entity>(); }
        }

        public Attributes attributes
        {
            get { return GetComponent<Attributes>(); }
        }

        private void Awake()
        {
            iBehavior = Instantiate(idlePrefab);
            iBehavior.behavior = this;
        }
    }
}
