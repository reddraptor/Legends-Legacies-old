using UnityEngine;
using Assets.Scripts.Components;
using System.Collections.Generic;


namespace Assets.Scripts.Managers
{
    public class BehaviorManager : MonoBehaviour
    {

        internal System.Random randomizer;

        internal EntityManager entityManager;
        internal MovementManager movementManager;
        internal WorldManager worldManager;

        private void Awake()
        {
            randomizer = new System.Random();
            entityManager = GetComponent<EntityManager>();
            movementManager = GetComponent<MovementManager>();
            worldManager = GetComponent<WorldManager>();
        }

        internal void RunBehaviors()
        {
            List<Behavior> behaviorList = GetBehaviorList();

            // Run behaviors
            foreach (Behavior behavior in behaviorList)
            {
                if (behavior.attributes.HitPoints < 0)
                {
                    entityManager.Despawn(behavior.entity);
                }
                else
                {
                    if (behavior) behavior.idleBehavior.Run(this);
                }
            }
        }



        private void Update()
        {
            List<Behavior> behaviorList = GetBehaviorList();

            foreach (Behavior behavior in behaviorList)
            {
                if (behavior)
                    if (behavior.movement)
                        if (!behavior.movement.isActive) behavior.isIdle = true;
            }

        }


        private List<Behavior> GetBehaviorList()
        {
            Behavior behavior;
            List<Behavior> behaviorList = new List<Behavior>();

            foreach (Entity entity in entityManager.MobCollection)
            {
                if (entity)
                {
                    behavior = entity.GetComponent<Behavior>();
                    if (behavior) behaviorList.Add(behavior);
                }
            }
            return behaviorList;
        }

    }
}
