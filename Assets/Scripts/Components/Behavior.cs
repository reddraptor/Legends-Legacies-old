using UnityEngine;
using Assets.Scripts.ScriptableObjects.BehaviorScripts;

namespace Assets.Scripts.Components
{
    public class Behavior: MonoBehaviour
    {
        public IdleBehavior idlePrefab;

        internal enum State {Idle, Moving}

        internal IdleBehavior idleBehavior;

        internal Movable movable;

        internal EntityMember entityMember;

        internal Attributes attributes;

        internal State state = State.Idle;

        private void Start()
        {
            entityMember = GetComponent<EntityMember>();
            movable = GetComponent<Movable>();
            entityMember = GetComponent<EntityMember>();
            attributes = GetComponent<Attributes>();

            idleBehavior = Instantiate(idlePrefab);
            idleBehavior.behavior = this;
        }
    }
}
