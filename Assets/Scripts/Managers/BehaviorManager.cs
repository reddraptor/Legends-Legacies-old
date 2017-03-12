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

        private List<Behavior> behaviorList;
        
        private void Awake()
        {
            behaviorList = new List<Behavior>();
            randomizer = new System.Random();
        }

        

        private void Update()
        {
        }

        public void RunBehaviors()
        {
            // Get list of behaviors from entity collections.
            foreach (Entity entity in entityManager.mobCollection)
            {
                behaviorList.Add(entity.GetComponent<Behavior>());
            }

            // Run behaviors
            foreach (Behavior behavior in behaviorList)
            {
                behavior.idleBehavior.Run(this);
            }

            // Clear list
            behaviorList.Clear();
        }
    }
}
