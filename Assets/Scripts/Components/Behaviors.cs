using UnityEngine;
using Assets.Scripts.ScriptableObjects.BehaviorScripts;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    public class Behaviors: MonoBehaviour
    {
        public IdleBehavior idleBehavior;

        internal enum State {Idle, Moving}

        internal Movable movable;

        internal EntityMember entityMember;

        internal Attributes attributes;

        internal State state = State.Idle;

        private void Start()
        {
            entityMember = GetComponent<EntityMember>();
            movable = GetComponent<Movable>();
            attributes = GetComponent<Attributes>();
        }
    }
}
