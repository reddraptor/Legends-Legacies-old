using UnityEngine;
using Assets.Scripts.Components;
using System.Collections.Generic;


namespace Assets.Scripts.Managers
{
    public class BehaviorManager : MonoBehaviour
    {

        public System.Random randomizer;

        public EntityManager entityManager 
        {
            get { return GetComponent<EntityManager>(); }
        }

        public MovementManager movementManager
        {
            get { return GetComponent<MovementManager>(); }
        }

        public WorldManager worldManager
        {
            get { return GetComponent<WorldManager>(); }
        }

        private void Awake()
        {
            randomizer = new System.Random();
        }

        

        private void Update()
        {
            List<Behavior> behaviorList = GetBehaviorList();

            foreach (Behavior behavior in behaviorList)
            {
                if (behavior)
                    if (behavior.movement)
                        if (!behavior.movement.isMoving) behavior.isIdle = true;
            }

        }

        public void RunBehaviors()
        {
            List<Behavior> behaviorList = GetBehaviorList();

            // Run behaviors
            foreach (Behavior behavior in behaviorList)
            {
                if (behavior) behavior.idleBehavior.Run(this);
            }
        }

        private List<Behavior> GetBehaviorList()
        {
            Behavior behavior;
            List<Behavior> behaviorList = new List<Behavior>();

            foreach (Entity entity in entityManager.mobCollection)
            {
                behavior = entity.GetComponent<Behavior>();
                if (behavior) behaviorList.Add(behavior);
            }
            return behaviorList;
        }

    }
}
