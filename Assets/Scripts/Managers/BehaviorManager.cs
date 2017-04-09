using UnityEngine;
using Assets.Scripts.Components;
using Assets.Scripts.EventHandlers;
using Assets.Scripts.Data_Types;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Managers
{
    public class BehaviorManager : MonoBehaviour, IMovementEventHandler
    {

        internal System.Random randomizer = new System.Random();

        internal EntityManager entityManager;
        internal MovementManager movementManager;
        internal WorldManager worldManager;

        internal void RunBehaviors()
        {
            List<Behavior> behaviorList = GetBehaviorList();

            // Run behaviors
            foreach (Behavior behavior in behaviorList)
            {
                if (behavior.attributes && behavior.attributes.HitPoints < 0 && behavior.entityMember)
                {
                    entityManager.Despawn(behavior.entityMember);
                }
                else if (behavior.state == Behavior.State.Idle && behavior.idleBehavior)
                {
                    behavior.idleBehavior.Run(this);
                }
            }
        }

        private void Start()
        {
            entityManager = GetComponent<EntityManager>();
            movementManager = GetComponent<MovementManager>();
            worldManager = GetComponent<WorldManager>();
        }


        private List<Behavior> GetBehaviorList()
        {
            Behavior behavior;
            List<Behavior> behaviorList = new List<Behavior>();

            foreach (EntityMember entityMember in entityManager.mobCollection.Members)
            {
                if (entityMember)
                {
                    behavior = entityMember.GetComponent<Behavior>();
                    if (behavior) behaviorList.Add(behavior);
                }
            }
            return behaviorList;
        }


        public Behavior GetBehavior(EntityMember entityMember)
        {
            return entityMember.GetComponent<Behavior>();
        }

        public void OnMovementStart(Movement movement)
        {
            // Do nothing
            //Debug.Log("Started: " + movement);
        }

        public void OnMovementEnd(Movement movement)
        {
            //Debug.Log("Ended: " + movement);
            Behavior behavior = null;
            if (movement.movable.entity is EntityMember) behavior = GetBehavior((EntityMember)movement.movable.entity);
            if (behavior) behavior.state = Behavior.State.Idle;
        }

        public void OnMovementCreate(Movement movement)
        {
            //Debug.Log("Created: " + movement);
        }

        public void OnMovementChange(Movement movement)
        {
            //Debug.Log("Changed: " + movement);
        }
    }
}
