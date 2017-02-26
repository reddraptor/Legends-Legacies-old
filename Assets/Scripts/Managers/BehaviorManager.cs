using UnityEngine;
using Assets.Scripts.Components;
using System.Collections.Generic;


namespace Assets.Scripts.Managers
{
    public class BehaviorManager : MonoBehaviour
    {

        public EntityManager entityManager 
        {
            get { return eManager; }
        }

        public MovementManager movementManager
        {
            get { return mManager; }
        }


        private List<Behavior> behaviorList;
        EntityManager eManager;
        MovementManager mManager;






        private void Awake()
        {
            behaviorList = new List<Behavior>();
            eManager = GetComponent<EntityManager>();
            mManager = GetComponent<MovementManager>();
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
